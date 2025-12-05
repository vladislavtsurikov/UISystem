using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class CsGenerator
    {
        public static int DefaultTabSize = 4;
        public static string IndentSingle => new(' ', DefaultTabSize);
        public string Path { get; set; }
        public List<FileModel> Files { get; set; } = new();

        public void CreateFiles()
        {
            if (!Directory.Exists(Path))
            {
                var message = "Path not valid: " + Path;
                Console.WriteLine(message);
                throw new InvalidOperationException(message);
            }

            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            foreach (FileModel file in Files)
            {
                var filePath = $@"{Path}\{file.FullName}";
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.Write(file);
                }
            }

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}
