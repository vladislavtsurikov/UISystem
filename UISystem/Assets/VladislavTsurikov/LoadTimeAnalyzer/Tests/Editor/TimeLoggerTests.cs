using System.Threading.Tasks;
using NUnit.Framework;
using VladislavTsurikov.LoadTimeAnalyzer.Runtime;

namespace VladislavTsurikov.LoadTimeAnalyzer.Tests.Editor
{
    public class TimeLoggerTests
    {
        /*[SetUp]
        public void SetUp()
        {
            TimeLogger.Clear();
        }

        [Test]
        public async Task TimeLogger_CapturesNestedTimingCorrectly()
        {
            TimeLogger.BeginSample("Root");

            await Task.Delay(50);

            TimeLogger.BeginSample("Child A");
            await Task.Delay(30);
            TimeLogger.EndSample("Child A");

            TimeLogger.BeginSample("Child B");
            await Task.Delay(20);
            TimeLogger.EndSample("Child B");

            TimeLogger.EndSample("Root"); // Root

            System.Type loggerType = typeof(TimeLogger);
            System.Reflection.FieldInfo treeField = loggerType.GetField("_tree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            TimeLogTree tree = (TimeLogTree)treeField.GetValue(null);

            Assert.That(tree.Roots.Count, Is.EqualTo(1));
            Assert.That(tree.Leaves.Count, Is.EqualTo(2));

            SampleNode root = tree.Roots[0];
            SampleNode childA = root.Children[0];
            SampleNode childB = root.Children[1];

            Assert.That(root.ElapsedMs, Is.GreaterThan(childA.ElapsedMs + childB.ElapsedMs));
            Assert.That(childA.Parent, Is.EqualTo(root));
            Assert.That(childB.Parent, Is.EqualTo(root));

            TimeLogger.EnableSorting(true);
            TimeLogger.LogResults();
        }

        [Test]
        public void TimeLogger_ClearsStateProperly()
        {
            TimeLogger.BeginSample("Root");
            TimeLogger.EndSample("Root");

            TimeLogger.Clear();

            System.Type loggerType = typeof(TimeLogger);
            System.Reflection.FieldInfo treeField = loggerType.GetField("_tree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            TimeLogTree tree = (TimeLogTree)treeField.GetValue(null);

            Assert.That(tree, Is.Null);
        }*/
    }
}
