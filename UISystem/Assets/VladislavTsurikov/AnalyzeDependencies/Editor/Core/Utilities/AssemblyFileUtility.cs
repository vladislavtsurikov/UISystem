using System.Collections.Generic;
using System.IO;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core.Utilities
{
    public static class AssemblyFileUtility
    {
        public static List<string> GetCSharpFilesForAssembly(string asmdefPath)
        {
            List<string> csFiles = new List<string>();
            string directoryPath = Path.GetDirectoryName(asmdefPath);

            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
                csFiles.AddRange(files);
            }

            return csFiles;
        }
    }
}
