using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.Core
{
    public static class DependencyAnalyzerInitialize
    {
        private static DependencyAnalyzer _instance;
        private static DependencyToolManager _toolManager;
        private static bool _initialized;

        public static DependencyAnalyzer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DependencyAnalyzer();
                }
                return _instance;
            }
        }

        [DidReloadScripts]
        private static void OnReloadScripts()
        {
            if (_initialized)
                return;

            _initialized = true;
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            try
            {
                Debug.Log("[AnalyzeDependencies] Starting initialization...");

                var analyzer = Instance;

                // Core: Build assembly database
                analyzer.BuildAssemblyDatabase();
                Debug.Log($"[AnalyzeDependencies] Scanned {analyzer.GetAllAssemblies().Count} assemblies");

                _toolManager = new DependencyToolManager();

                // Call Setup on all tools
                foreach (var tool in _toolManager.GetTools())
                {
                    tool.Setup(analyzer);
                }

                Debug.Log("[AnalyzeDependencies] Initialization complete");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AnalyzeDependencies] Initialization failed: {e.Message}");
            }
        }
    }
}
