using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.ToolSystem.Runtime.Core;

namespace VladislavTsurikov.PackageJsonSync.Editor.ToolSystem
{
    [Name("Package Json Sync/Generate package.json")]
    [Tool("Generate package.json", "Generate UPM package.json with external dependencies")]
    [ToolGroup("PackageJson")]
    public class GeneratePackageJsonTool : EditorTool
    {
        private PackageJsonGenerator _generator;
        private string _lastRepositoryPath;

        protected override void OnSetupTool()
        {
            // OnSetupTool does nothing for export tools
            // Generation is triggered from UI button
            _generator = new PackageJsonGenerator();
        }

        public void Generate()
        {
            // Ask user to select repository
            string selectedPath = EditorUtility.OpenFolderPanel("Select Repository Folder", Application.dataPath, "");

            if (string.IsNullOrEmpty(selectedPath))
                return;

            if (!Directory.Exists(Path.Combine(selectedPath, ".git")))
            {
                EditorUtility.DisplayDialog("Error", "Selected folder is not a git repository.", "OK");
                return;
            }

            _lastRepositoryPath = selectedPath;

            try
            {
                EditorUtility.DisplayProgressBar("Scanning", "Scanning repository for assemblies...", 0.3f);

                _generator.ScanRepository(_lastRepositoryPath);

                if (_generator.AssemblyCount == 0)
                {
                    EditorUtility.ClearProgressBar();
                    EditorUtility.DisplayDialog("No Assemblies", "No assembly definitions found in the selected repository.", "OK");
                    return;
                }

                EditorUtility.DisplayProgressBar("Analyzing", "Collecting external dependencies...", 0.6f);

                var dependencies = _generator.CollectExternalDependencies();

                EditorUtility.ClearProgressBar();

                if (dependencies.Count > 0)
                {
                    var message = $"Found {dependencies.Count} external dependencies:\n\n";
                    message += string.Join("\n", dependencies.Take(10).Select(kvp => $"• {kvp.Key}"));
                    if (dependencies.Count > 10)
                    {
                        message += $"\n... and {dependencies.Count - 10} more";
                    }
                    message += "\n\nGenerate package.json?";

                    if (EditorUtility.DisplayDialog("Generate package.json", message, "Generate", "Cancel"))
                    {
                        EditorUtility.DisplayProgressBar("Generating", "Writing package.json...", 0.8f);
                        _generator.GeneratePackageJson(dependencies);

                        EditorUtility.ClearProgressBar();
                        EditorUtility.DisplayDialog("Success",
                            $"package.json generated successfully at:\n{_lastRepositoryPath}/package.json",
                            "OK");

                        AssetDatabase.Refresh();
                        EditorUtility.RevealInFinder(Path.Combine(_lastRepositoryPath, "package.json"));
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("No External Dependencies",
                        "No external dependencies found. All references are internal to VladislavTsurikov namespace.",
                        "OK");
                }
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to generate package.json: {e.Message}", "OK");
                Debug.LogError($"[PackageJsonSync] Error: {e}");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
