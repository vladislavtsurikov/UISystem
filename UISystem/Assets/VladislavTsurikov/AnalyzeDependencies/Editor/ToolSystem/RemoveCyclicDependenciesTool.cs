using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Graph;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [Name("Dependency Analyzer/Remove Cyclic Dependencies")]
    [Tool("Remove Cyclic Dependencies", "Break circular dependency chains by removing dependencies")]
    [ToolGroup("Dependencies")]
    public class RemoveCyclicDependenciesTool : EditorTool
    {
        private List<CyclicDependency> _cycles = new List<CyclicDependency>();

        public List<CyclicDependency> Cycles => _cycles;

        protected override void OnSetupTool()
        {
            // OnSetupTool only detects cycles, no UI interaction
            DetectCycles();
        }

        private void DetectCycles()
        {
            var analyzer = DependencyAnalyzerInitialize.Instance;
            var detector = new CyclicDependencyDetector(analyzer);
            _cycles = detector.DetectCycles();

            if (_cycles.Count == 0)
            {
                Debug.Log("No cyclic dependencies detected. Your assembly dependency graph is acyclic (DAG).");
            }
            else
            {
                Debug.Log($"Detected {_cycles.Count} cyclic dependency chain(s).");
            }
        }

        public void BreakCycles()
        {
            if (_cycles.Count == 0)
            {
                EditorUtility.DisplayDialog("No Cycles Found",
                    "✓ No cyclic dependencies detected!\n\nYour assembly dependency graph is acyclic (DAG).",
                    "OK");
                return;
            }

            string message = $"Found {_cycles.Count} cyclic dependency chain(s).\n\n" +
                             "These cycles can cause:\n" +
                             "- Increased compilation times\n" +
                             "- Difficulty in code maintenance\n" +
                             "- Potential runtime issues\n\n" +
                             "Remove dependencies to break cycles? This action cannot be undone (except through version control).";

            if (!EditorUtility.DisplayDialog("Remove Cyclic Dependencies", message, "Remove", "Cancel"))
            {
                return;
            }

            EditorUtility.DisplayProgressBar("Removing", "Breaking cyclic dependencies...", 0.5f);

            try
            {
                var analyzer = DependencyAnalyzerInitialize.Instance;
                int removedCount = RemoveCyclicDependencies(analyzer, _cycles);

                EditorUtility.ClearProgressBar();

                if (removedCount > 0)
                {
                    EditorUtility.DisplayDialog("Complete",
                        $"Removed {removedCount} dependencies to break {_cycles.Count} cycle(s).\n\n" +
                        "Re-analyzing to verify cycles are resolved...",
                        "OK");

                    analyzer.BuildAssemblyDatabase();
                    DetectCycles(); // Re-analyze
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

        private int RemoveCyclicDependencies(DependencyAnalyzer analyzer, List<CyclicDependency> cycles)
        {
            int removedCount = 0;
            var processedAssemblies = new HashSet<string>();

            foreach (var cycle in cycles)
            {
                if (cycle.Chain.Count < 2)
                    continue;

                string firstAssembly = cycle.Chain[0];
                string secondAssembly = cycle.Chain[1];

                if (processedAssemblies.Contains($"{firstAssembly}->{secondAssembly}"))
                    continue;

                if (RemoveDependency(analyzer, firstAssembly, secondAssembly))
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

        private bool RemoveDependency(DependencyAnalyzer analyzer, string fromAssembly, string toAssembly)
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

                if (asmdefData.references == null || asmdefData.references.Length == 0)
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
    }
}
