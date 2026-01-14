using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AnalyzeDependencies.Editor.Core.Models;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core
{
    public class DependencyAnalyzer
    {
        private readonly Dictionary<string, string> _guidToName = new Dictionary<string, string>();
        private readonly Dictionary<string, AssemblyInfo> _assemblies = new Dictionary<string, AssemblyInfo>();

        public List<AssemblyInfo> GetAllAssemblies() => _assemblies.Values.ToList();

        public List<AssemblyInfo> GetSelectedAssemblies() => _assemblies.Values.Where(a => a.IsSelected).ToList();

        public void SelectAll()
        {
            foreach (var assembly in _assemblies.Values)
            {
                assembly.IsSelected = true;
            }
        }

        public void DeselectAll()
        {
            foreach (var assembly in _assemblies.Values)
            {
                assembly.IsSelected = false;
            }
        }

        public void SelectAssemblies(List<AssemblyInfo> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                assembly.IsSelected = true;
            }
        }

        public void DeselectAssemblies(List<AssemblyInfo> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                assembly.IsSelected = false;
            }
        }

        public Dictionary<string, AssemblyInfo> GetAssembliesDictionary() => _assemblies;

        public Dictionary<string, string> GetGuidToNameMap() => _guidToName;

        public void BuildAssemblyDatabase()
        {
            _guidToName.Clear();
            _assemblies.Clear();

            string[] asmdefGuids = AssetDatabase.FindAssets("t:AssemblyDefinitionAsset");

            foreach (string guid in asmdefGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                try
                {
                    string json = File.ReadAllText(path);
                    AssemblyDefinitionData asmdefData = JsonUtility.FromJson<AssemblyDefinitionData>(json);

                    if (string.IsNullOrEmpty(asmdefData.name))
                        continue;

                    _guidToName[guid] = asmdefData.name;

                    var assemblyInfo = new AssemblyInfo
                    {
                        Name = asmdefData.name,
                        Guid = guid,
                        Path = path
                    };

                    if (asmdefData.references != null)
                    {
                        foreach (string reference in asmdefData.references)
                        {
                            string refGuid = reference.StartsWith("GUID:") ? reference.Substring(5) : reference;
                            assemblyInfo.Dependencies.Add(refGuid);
                        }
                    }

                    _assemblies[asmdefData.name] = assemblyInfo;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Failed to parse {path}: {e.Message}");
                }
            }

            Debug.Log($"Found {_assemblies.Count} assemblies");
        }

        public string GetAssemblyNameByGuid(string guid) => _guidToName.ContainsKey(guid) ? _guidToName[guid] : guid;
    }
}
