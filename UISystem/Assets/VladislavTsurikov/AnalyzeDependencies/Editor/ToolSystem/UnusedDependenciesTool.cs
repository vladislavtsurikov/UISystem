using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Utilities;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.ToolSystem.Runtime.Core;
using VladislavTsurikov.ToolSystem.Runtime.Core.Attributes;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [Name("Dependency Analyzer/Remove Unused Dependencies")]
    [Tool("Remove Unused Dependencies", "Analyze and remove unused assembly dependencies")]
    [ToolGroup("Dependencies")]
    public class UnusedDependenciesTool : EditorTool
    {
        private List<AssemblyInfo> _assembliesToProcess = new List<AssemblyInfo>();
        private int _totalUnusedCount;

        public List<AssemblyInfo> AssembliesToProcess => _assembliesToProcess;
        public int TotalUnusedCount => _totalUnusedCount;

        protected override void OnSetupTool()
        {
            // OnSetupTool only analyzes data, no UI interaction
            AnalyzeUnusedDependencies();
        }

        private void AnalyzeUnusedDependencies()
        {
            var analyzer = DependencyAnalyzerInitialize.Instance;
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

            _assembliesToProcess = assemblies.Where(a => a.UnusedDependencies.Count > 0).ToList();
            _totalUnusedCount = totalUnused;

            Debug.Log($"Analysis complete. Found {totalUnused} unused dependencies across {_assembliesToProcess.Count} assemblies");
        }

        public void RemoveUnusedDependencies()
        {
            if (_assembliesToProcess.Count == 0)
            {
                EditorUtility.DisplayDialog("No Unused Dependencies", "No unused dependencies found.", "OK");
                return;
            }

            string message = $"Found {_totalUnusedCount} unused dependencies in {_assembliesToProcess.Count} assembly(ies).\n\nRemove them? This action cannot be undone (except through version control).";

            if (!EditorUtility.DisplayDialog("Remove Unused Dependencies", message, "Remove", "Cancel"))
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Removing", "Removing unused dependencies...", 0.5f);

            try
            {
                int removedCount = 0;

                foreach (var assembly in _assembliesToProcess)
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

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Complete", $"Removed {removedCount} unused dependencies from {_assembliesToProcess.Count} assembly(ies).", "OK");

                // Re-analyze after removal
                var analyzer = DependencyAnalyzerInitialize.Instance;
                analyzer.BuildAssemblyDatabase();
                AnalyzeUnusedDependencies();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
