using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Utilities;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Tools
{
    public class UnusedDependenciesTool : DependencyTool
    {
        public override string Name => "Remove Unused Dependencies";
        public override string Description => "Analyze and remove unused dependencies";

        public override bool CanExecute(DependencyAnalyzer analyzer)
        {
            return analyzer.GetAllAssemblies().Count > 0;
        }

        public override void Setup(DependencyAnalyzer analyzer)
        {
            AnalyzeUnusedDependencies(analyzer);
            int totalUnused = 0;
            foreach (var assembly in analyzer.GetAllAssemblies())
            {
                totalUnused += assembly.UnusedDependencies.Count;
            }
            Debug.Log($"[AnalyzeDependencies] Found {totalUnused} unused dependencies");
        }

        public override void Execute(DependencyAnalyzer analyzer)
        {
            EditorUtility.DisplayProgressBar("Analyzing", "Analyzing unused dependencies...", 0.3f);

            try
            {
                AnalyzeUnusedDependencies(analyzer);

                var assemblies = analyzer.GetSelectedAssemblies();
                bool workingWithSelected = assemblies.Count > 0;

                if (!workingWithSelected)
                    assemblies = analyzer.GetAllAssemblies();

                var assembliesToProcess = assemblies.Where(a => a.UnusedDependencies.Count > 0).ToList();

                if (assembliesToProcess.Count == 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("No Unused Dependencies", "No unused dependencies found.", "OK");
                    return;
                }

                int totalUnused = assembliesToProcess.Sum(a => a.UnusedDependencies.Count);
                string message = workingWithSelected
                    ? $"Found {totalUnused} unused dependencies in {assembliesToProcess.Count} selected assembly(ies).\n\nRemove them? This action cannot be undone (except through version control)."
                    : $"Found {totalUnused} unused dependencies in {assembliesToProcess.Count} assembly(ies).\n\nRemove them? This action cannot be undone (except through version control).";

                EditorUtility.ClearProgressBar();

                if (!EditorUtility.DisplayDialog("Remove Unused Dependencies", message, "Remove", "Cancel"))
                {
                    return;
                }

                EditorUtility.DisplayProgressBar("Removing", "Removing unused dependencies...", 0.7f);

                int removedCount = RemoveUnusedDependencies(assembliesToProcess);

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Complete", $"Removed {removedCount} unused dependencies from {assembliesToProcess.Count} assembly(ies).", "OK");

                analyzer.BuildAssemblyDatabase();
                AnalyzeUnusedDependencies(analyzer);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private int RemoveUnusedDependencies(List<AssemblyInfo> assemblies)
        {
            int removedCount = 0;

            foreach (var assembly in assemblies)
            {
                if (assembly.UnusedDependencies.Count == 0)
                    continue;

                try
                {
                    string asmdefPath = assembly.Path;
                    string json = File.ReadAllText(asmdefPath);
                    AssemblyDefinitionData asmdefData = JsonUtility.FromJson<AssemblyDefinitionData>(json);

                    var originalCount = asmdefData.references.Count;
                    var newReferences = new List<string>();

                    foreach (string reference in asmdefData.references)
                    {
                        string refGuid = reference.StartsWith("GUID:") ? reference.Substring(5) : reference;

                        if (!assembly.UnusedDependencies.Contains(refGuid))
                        {
                            newReferences.Add(reference);
                        }
                    }

                    if (newReferences.Count < originalCount)
                    {
                        asmdefData.references = newReferences;
                        string newJson = JsonUtility.ToJson(asmdefData, true);
                        File.WriteAllText(asmdefPath, newJson);

                        removedCount += (originalCount - newReferences.Count);
                        Debug.Log($"Updated {assembly.Name}: removed {originalCount - newReferences.Count} dependencies");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to update {assembly.Name}: {e.Message}");
                }
            }

            if (removedCount > 0)
            {
                AssetDatabase.Refresh();
            }

            return removedCount;
        }

        private void AnalyzeUnusedDependencies(DependencyAnalyzer analyzer)
        {
            int totalUnused = 0;
            var assemblies = analyzer.GetAllAssemblies();
            var guidToName = analyzer.GetGuidToNameMap();

            foreach (var assembly in assemblies)
            {
                assembly.UnusedDependencies.Clear();

                if (assembly.Dependencies.Count == 0)
                    continue;

                List<string> csFiles = AssemblyFileUtility.GetCSharpFilesForAssembly(assembly.Path);

                foreach (string depGuid in assembly.Dependencies)
                {
                    if (!guidToName.ContainsKey(depGuid))
                        continue;

                    string depName = guidToName[depGuid];
                    List<string> namespaces = NamespaceUtility.GetNamespacesFromAssemblyName(depName);

                    bool isUsed = NamespaceUtility.IsNamespaceUsedInFiles(csFiles, namespaces);

                    if (!isUsed)
                    {
                        assembly.UnusedDependencies.Add(depGuid);
                        totalUnused++;
                    }
                }
            }

            Debug.Log($"Analysis complete. Found {totalUnused} unused dependencies across {assemblies.Count(a => a.UnusedDependencies.Count > 0)} assemblies");
        }
    }
}
