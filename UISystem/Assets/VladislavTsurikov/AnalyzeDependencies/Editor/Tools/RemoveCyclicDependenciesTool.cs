using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Tools
{
    public class RemoveCyclicDependenciesTool : DependencyTool
    {
        private List<CyclicDependency> _cyclicDependencies = new List<CyclicDependency>();

        public override string Name => "Remove Cyclic Dependencies";
        public override string Description => "Break circular dependency chains by removing dependencies";

        public List<CyclicDependency> GetCyclicDependencies() => _cyclicDependencies;

        public override bool CanExecute(DependencyAnalyzer analyzer)
        {
            return _cyclicDependencies.Count > 0;
        }

        public override void Setup(DependencyAnalyzer analyzer)
        {
            DetectCyclicDependencies(analyzer);
            var cycles = _cyclicDependencies;
            if (cycles.Count > 0)
            {
                Debug.LogWarning($"[AnalyzeDependencies] Found {cycles.Count} cyclic dependency chains");
            }
            else
            {
                Debug.Log("[AnalyzeDependencies] No cyclic dependencies detected");
            }
        }

        public override void Execute(DependencyAnalyzer analyzer)
        {
            EditorUtility.DisplayProgressBar("Analyzing", "Detecting cyclic dependencies...", 0.3f);

            try
            {
                DetectCyclicDependencies(analyzer);
                var cycles = _cyclicDependencies;

                if (cycles.Count == 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("No Cycles Found",
                        "✓ No cyclic dependencies detected!\n\nYour assembly dependency graph is acyclic (DAG).",
                        "OK");
                    return;
                }

                EditorUtility.ClearProgressBar();

                string message = $"Found {cycles.Count} cyclic dependency chain(s).\n\n" +
                                 "These cycles can cause:\n" +
                                 "- Increased compilation times\n" +
                                 "- Difficulty in code maintenance\n" +
                                 "- Potential runtime issues\n\n" +
                                 "Remove dependencies to break cycles? This action cannot be undone (except through version control).";

                if (!EditorUtility.DisplayDialog("Remove Cyclic Dependencies", message, "Remove", "Cancel"))
                {
                    return;
                }

                EditorUtility.DisplayProgressBar("Removing", "Breaking cyclic dependencies...", 0.7f);

                int removedCount = RemoveCyclicDependencies(cycles, analyzer);

                EditorUtility.ClearProgressBar();

                if (removedCount > 0)
                {
                    EditorUtility.DisplayDialog("Complete",
                        $"Removed {removedCount} dependencies to break {cycles.Count} cycle(s).\n\n" +
                        "Re-analyzing to verify cycles are resolved...",
                        "OK");

                    analyzer.BuildAssemblyDatabase();
                    analyzer.DetectCyclicDependencies();

                    var remainingCycles = analyzer.GetCyclicDependencies();
                    if (remainingCycles.Count == 0)
                    {
                        Debug.Log("✓ All cyclic dependencies successfully resolved!");
                    }
                    else
                    {
                        Debug.LogWarning($"Warning: {remainingCycles.Count} cycle(s) remain. Manual intervention may be required.");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("No Changes",
                        "No dependencies were removed. Manual intervention may be required.",
                        "OK");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private int RemoveCyclicDependencies(List<CyclicDependency> cycles, DependencyAnalyzer analyzer)
        {
            int removedCount = 0;
            var processedAssemblies = new HashSet<string>();

            foreach (var cycle in cycles)
            {
                if (cycle.Cycle.Count < 2)
                    continue;

                string firstAssembly = cycle.Cycle[0];
                string secondAssembly = cycle.Cycle[1];

                if (processedAssemblies.Contains($"{firstAssembly}->{secondAssembly}"))
                    continue;

                if (RemoveDependency(firstAssembly, secondAssembly, analyzer))
                {
                    removedCount++;
                    processedAssemblies.Add($"{firstAssembly}->{secondAssembly}");
                    Debug.Log($"Removed dependency: {firstAssembly} -> {secondAssembly}");
                }
            }

            if (removedCount > 0)
            {
                AssetDatabase.Refresh();
            }

            return removedCount;
        }

        private bool RemoveDependency(string fromAssembly, string toAssembly, DependencyAnalyzer analyzer)
        {
            try
            {
                var assemblies = analyzer.GetAssembliesDictionary();

                if (!assemblies.ContainsKey(fromAssembly))
                    return false;

                var assembly = assemblies[fromAssembly];
                string asmdefPath = assembly.Path;

                if (!File.Exists(asmdefPath))
                    return false;

                string json = File.ReadAllText(asmdefPath);
                AssemblyDefinitionData asmdefData = JsonUtility.FromJson<AssemblyDefinitionData>(json);

                if (asmdefData.references == null || asmdefData.references.Count == 0)
                    return false;

                var guidToName = analyzer.GetGuidToNameMap();
                var targetGuid = guidToName.FirstOrDefault(kvp => kvp.Value == toAssembly).Key;

                if (string.IsNullOrEmpty(targetGuid))
                    return false;

                var newReferences = new List<string>();
                bool removed = false;

                foreach (string reference in asmdefData.references)
                {
                    string refGuid = reference.StartsWith("GUID:") ? reference.Substring(5) : reference;

                    if (refGuid == targetGuid)
                    {
                        removed = true;
                        continue;
                    }

                    newReferences.Add(reference);
                }

                if (removed)
                {
                    asmdefData.references = newReferences;
                    string newJson = JsonUtility.ToJson(asmdefData, true);
                    File.WriteAllText(asmdefPath, newJson);
                    return true;
                }

                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to remove dependency from {fromAssembly}: {e.Message}");
                return false;
            }
        }

        private void DetectCyclicDependencies(DependencyAnalyzer analyzer)
        {
            var detector = new CyclicDependencyDetector(analyzer);
            _cyclicDependencies = detector.DetectCycles();
        }
    }
}
