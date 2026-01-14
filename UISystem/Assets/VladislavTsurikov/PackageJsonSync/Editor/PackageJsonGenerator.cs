using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.PackageJsonSync.Editor
{
    public class PackageJsonGenerator
    {
        private readonly Dictionary<string, AssemblyDefinitionData> _assemblies = new Dictionary<string, AssemblyDefinitionData>();
        private string _repositoryPath;

        public string RepositoryPath => _repositoryPath;
        public int AssemblyCount => _assemblies.Count;

        public void ScanRepository(string repositoryPath)
        {
            _repositoryPath = repositoryPath;
            _assemblies.Clear();

            string[] asmdefFiles = Directory.GetFiles(repositoryPath, "*.asmdef", SearchOption.AllDirectories);

            foreach (string asmdefPath in asmdefFiles)
            {
                try
                {
                    string json = File.ReadAllText(asmdefPath);
                    var asmdefData = JsonUtility.FromJson<AssemblyDefinitionData>(json);

                    if (asmdefData != null && !string.IsNullOrEmpty(asmdefData.name))
                    {
                        asmdefData.path = asmdefPath;
                        _assemblies[asmdefData.name] = asmdefData;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to parse {asmdefPath}: {e.Message}");
                }
            }
        }

        public Dictionary<string, string> CollectExternalDependencies()
        {
            var externalDeps = new HashSet<string>();
            var guidToNameMap = BuildGuidToNameMap();

            foreach (var assembly in _assemblies.Values)
            {
                if (assembly.references == null)
                    continue;

                foreach (string reference in assembly.references)
                {
                    string referenceName = null;

                    if (reference.StartsWith("GUID:"))
                    {
                        string guid = reference.Substring(5);
                        guidToNameMap.TryGetValue(guid, out referenceName);
                    }
                    else
                    {
                        referenceName = reference;
                    }

                    if (!string.IsNullOrEmpty(referenceName) && !referenceName.StartsWith("VladislavTsurikov."))
                    {
                        externalDeps.Add(referenceName);
                    }
                }
            }

            var dependencies = new Dictionary<string, string>();
            foreach (string depName in externalDeps.OrderBy(d => d))
            {
                string packageName = ConvertAssemblyNameToPackageName(depName);
                dependencies[packageName] = "1.0.0";
            }

            return dependencies;
        }

        public void GeneratePackageJson(Dictionary<string, string> externalDependencies)
        {
            if (string.IsNullOrEmpty(_repositoryPath))
            {
                throw new InvalidOperationException("Repository path not set. Call ScanRepository first.");
            }

            string packageJsonPath = Path.Combine(_repositoryPath, "package.json");
            string folderName = Path.GetFileName(_repositoryPath);
            string packageName = $"com.vladislavtsurikov.{ConvertToPackageName(folderName.ToLower())}";

            var packageData = new Dictionary<string, object>
            {
                { "name", packageName },
                { "version", "1.0.0" },
                { "displayName", "Universal Toolkit" },
                { "description", "Universal Toolkit for Unity" },
                { "unity", "2021.3" }
            };

            if (externalDependencies.Count > 0)
            {
                packageData["dependencies"] = externalDependencies;
            }

            string json = SerializePackageJson(packageData);
            File.WriteAllText(packageJsonPath, json, Encoding.UTF8);
        }

        private Dictionary<string, string> BuildGuidToNameMap()
        {
            var guidToName = new Dictionary<string, string>();

            string[] metaFiles = Directory.GetFiles(_repositoryPath, "*.asmdef.meta", SearchOption.AllDirectories);

            foreach (string metaPath in metaFiles)
            {
                string asmdefPath = metaPath.Substring(0, metaPath.Length - 5);
                if (File.Exists(asmdefPath))
                {
                    string guid = ExtractGuidFromMeta(metaPath);
                    if (!string.IsNullOrEmpty(guid))
                    {
                        try
                        {
                            string json = File.ReadAllText(asmdefPath);
                            var asmdefData = JsonUtility.FromJson<AssemblyDefinitionData>(json);
                            if (asmdefData != null && !string.IsNullOrEmpty(asmdefData.name))
                            {
                                guidToName[guid] = asmdefData.name;
                            }
                        }
                        catch { }
                    }
                }
            }

            return guidToName;
        }

        private string ExtractGuidFromMeta(string metaPath)
        {
            try
            {
                string[] lines = File.ReadAllLines(metaPath);
                foreach (string line in lines)
                {
                    if (line.StartsWith("guid:"))
                    {
                        return line.Substring(5).Trim();
                    }
                }
            }
            catch { }

            return null;
        }

        private string ConvertAssemblyNameToPackageName(string assemblyName)
        {
            return assemblyName.Replace(".", "-").ToLower();
        }

        private string ConvertToPackageName(string moduleName)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < moduleName.Length; i++)
            {
                char c = moduleName[i];

                if (char.IsUpper(c) && i > 0)
                {
                    result.Append('-');
                }

                result.Append(char.ToLower(c));
            }

            return result.ToString();
        }

        private string SerializePackageJson(Dictionary<string, object> data)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");

            int index = 0;
            foreach (var kvp in data)
            {
                bool isLast = index == data.Count - 1;

                if (kvp.Value is Dictionary<string, string> dict)
                {
                    sb.AppendLine($"  \"{kvp.Key}\": {{");

                    int depIndex = 0;
                    foreach (var dep in dict.OrderBy(d => d.Key))
                    {
                        bool isDepLast = depIndex == dict.Count - 1;
                        sb.AppendLine($"    \"{dep.Key}\": \"{dep.Value}\"{(isDepLast ? "" : ",")}");
                        depIndex++;
                    }

                    sb.AppendLine($"  }}{(isLast ? "" : ",")}");
                }
                else
                {
                    sb.AppendLine($"  \"{kvp.Key}\": \"{kvp.Value}\"{(isLast ? "" : ",")}");
                }

                index++;
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        [Serializable]
        public class AssemblyDefinitionData
        {
            public string name;
            public string[] references;
            public string path;
        }
    }
}
