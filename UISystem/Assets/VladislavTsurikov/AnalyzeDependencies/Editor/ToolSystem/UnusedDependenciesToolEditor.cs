using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [ElementEditor(typeof(UnusedDependenciesTool))]
    public class UnusedDependenciesToolEditor : UIToolkitToolEditor
    {
        private UnusedDependenciesTool Tool => (UnusedDependenciesTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Analyzes your assemblies to find unused dependencies.\n\n" +
                "Removing unused dependencies can:\n" +
                "• Reduce compilation times\n" +
                "• Improve code maintainability\n" +
                "• Make dependency graphs cleaner",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Show analysis results
            if (Tool.AssembliesToProcess.Count > 0)
            {
                var resultsLabel = new Label($"Found {Tool.TotalUnusedCount} unused dependencies in {Tool.AssembliesToProcess.Count} assemblies:");
                resultsLabel.style.marginTop = 10;
                resultsLabel.style.marginBottom = 5;
                resultsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                container.Add(resultsLabel);

                // Show list of assemblies with unused dependencies
                var scrollView = new ScrollView(ScrollViewMode.Vertical);
                scrollView.style.maxHeight = 200;
                scrollView.style.marginBottom = 10;
                scrollView.style.backgroundColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f);
                scrollView.style.paddingTop = 5;
                scrollView.style.paddingBottom = 5;
                scrollView.style.paddingLeft = 5;
                scrollView.style.paddingRight = 5;

                foreach (var assembly in Tool.AssembliesToProcess.Take(20)) // Limit display to 20
                {
                    var assemblyLabel = new Label($"• {assembly.Name} ({assembly.UnusedDependencies.Count} unused)");
                    assemblyLabel.style.fontSize = 11;
                    scrollView.Add(assemblyLabel);
                }

                if (Tool.AssembliesToProcess.Count > 20)
                {
                    var moreLabel = new Label($"... and {Tool.AssembliesToProcess.Count - 20} more assemblies");
                    moreLabel.style.fontSize = 11;
                    moreLabel.style.color = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
                    scrollView.Add(moreLabel);
                }

                container.Add(scrollView);

                // Remove button
                var removeButton = new Button(() => Tool.RemoveUnusedDependencies());
                removeButton.text = "Remove Unused Dependencies";
                removeButton.style.height = 30;
                removeButton.style.marginTop = 5;
                container.Add(removeButton);
            }
            else
            {
                var noIssuesLabel = new Label("✓ No unused dependencies found!");
                noIssuesLabel.style.marginTop = 10;
                noIssuesLabel.style.color = new UnityEngine.Color(0.3f, 0.8f, 0.3f);
                noIssuesLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                container.Add(noIssuesLabel);
            }

            return container;
        }
    }
}
