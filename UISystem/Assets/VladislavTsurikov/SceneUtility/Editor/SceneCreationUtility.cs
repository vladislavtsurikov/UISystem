#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using VladislavTsurikov.Utility.Runtime;

namespace VladislavTsurikov.SceneUtility.Editor
{
    //This utility is required because Unity cannot create a Scene without automatically loading it into the level.
    public static class SceneCreationUtility
    {
        public static SceneAsset CreateScene(string sceneName, string pathToFolder, bool uniqueName = false)
        {
            var pathToScene = pathToFolder + "/" + sceneName + ".unity";
            Directory.CreateDirectory(Path.GetDirectoryName(pathToScene));

            SceneAsset sceneAsset = CreateScene(pathToScene);

            if (uniqueName)
            {
                var newName = sceneName + "_" + sceneAsset.GetHashCode().ToString().RemoveChar('-');

                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(sceneAsset), newName);
            }

            return sceneAsset;
        }

        private static SceneAsset CreateScene(string path)
        {
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (!sceneAsset)
            {
                const string template = "" +
                                        "%YAML 1.1\n" +
                                        "%TAG !u! tag:unity3d.com,2011:";

                if (!File.Exists(path) || File.ReadAllText(path) != template)
                {
                    Directory.GetParent(path).Create();
                    File.WriteAllText(path, template);
                }

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
                sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);

                if (true)
                {
                    sceneAsset.name = sceneAsset.name + "_" + sceneAsset.GetHashCode().ToString().RemoveChar('-');
                }

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            }

            return sceneAsset;
        }
    }
}
#endif
