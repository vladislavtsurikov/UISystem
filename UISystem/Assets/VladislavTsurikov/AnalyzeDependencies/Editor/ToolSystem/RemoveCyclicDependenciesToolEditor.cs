using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.ToolSystem.Editor.UIToolkit;

namespace VladislavTsurikov.AnalyzeDependencies.Editor.ToolSystem
{
    [ElementEditor(typeof(RemoveCyclicDependenciesTool))]
    public class RemoveCyclicDependenciesToolEditor : UIToolkitToolEditor
    {
        private RemoveCyclicDependenciesTool Tool => (RemoveCyclicDependenciesTool)Target;

        protected override VisualElement CreateVisualElement()
        {
            var container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 10;
            container.style.paddingRight = 10;

            var helpBox = new HelpBox(
                "Detects and breaks circular dependency chains.\n\n" +
                "Problems caused by cycles:\n" +
                "• Increased compilation times\n" +
                "• Harder to maintain code\n" +
                "• Potential runtime issues",
                HelpBoxMessageType.Info);
            container.Add(helpBox);

            // Show detection results
            if (Tool.Cycles.Count > 0)
            {
                var resultsLabel = new Label($"Found {Tool.Cycles.Count} cyclic dependency chain(s):");
                resultsLabel.style.marginTop = 10;
                resultsLabel.style.marginBottom = 5;
                resultsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                resultsLabel.style.color = new UnityEngine.Color(1.0f, 0.6f, 0.3f); // Orange warning color
                container.Add(resultsLabel);

                // Show list of cycles
                var scrollView = new ScrollView(ScrollViewMode.Vertical);
                scrollView.style.maxHeight = 200;
                scrollView.style.marginBottom = 10;
                scrollView.style.backgroundColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f);
                scrollView.style.paddingTop = 5;
                scrollView.style.paddingBottom = 5;
                scrollView.style.paddingLeft = 5;
                scrollView.style.paddingRight = 5;

                foreach (var cycle in Tool.Cycles.Take(15)) // Limit display to 15
                {
                    var cycleChain = string.Join(" → ", cycle.Cycle);
                    if (cycle.Cycle.Count > 0)
                    {
                        cycleChain += $" → {cycle.Cycle[0]}"; // Close the circle
                    }

                    var cycleLabel = new Label($"• {cycleChain}");
                    cycleLabel.style.fontSize = 10;
                    cycleLabel.style.whiteSpace = WhiteSpace.Normal;
                    cycleLabel.style.marginBottom = 3;
                    scrollView.Add(cycleLabel);
                }

                if (Tool.Cycles.Count > 15)
                {
                    var moreLabel = new Label($"... and {Tool.Cycles.Count - 15} more cycles");
                    moreLabel.style.fontSize = 11;
                    moreLabel.style.color = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
                    scrollView.Add(moreLabel);
                }

                container.Add(scrollView);

                // Break cycles button
                var breakButton = new Button(() => Tool.BreakCycles());
                breakButton.text = "Break Cyclic Dependencies";
                breakButton.style.height = 30;
                breakButton.style.marginTop = 5;
                breakButton.style.backgroundColor = new UnityEngine.Color(0.8f, 0.4f, 0.2f); // Orange button
                container.Add(breakButton);
            }
            else
            {
                var noIssuesLabel = new Label("✓ No cyclic dependencies found!");
                noIssuesLabel.style.marginTop = 10;
                noIssuesLabel.style.color = new UnityEngine.Color(0.3f, 0.8f, 0.3f);
                noIssuesLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
                container.Add(noIssuesLabel);

                var dagLabel = new Label("Your assembly dependency graph is acyclic (DAG).");
                dagLabel.style.marginTop = 5;
                dagLabel.style.fontSize = 11;
                dagLabel.style.color = new UnityEngine.Color(0.7f, 0.7f, 0.7f);
                container.Add(dagLabel);
            }

            return container;
        }
    }
}
