#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Events;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.SingleElementStackEditor;
using Actions_Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Events_Event = VladislavTsurikov.ActionFlow.Runtime.Events.Event;

namespace VladislavTsurikov.ActionFlow.Editor.Events
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : UnityEditor.Editor
    {
        private ReorderableListStackEditor<Actions_Action, ReorderableListComponentEditor> _actionsCollectionEditor;
        private SingleElementStackEditor<Events_Event, IMGUIElementEditor> _singleElementStackEditor;
        private Trigger Trigger => (Trigger)target;

        private void OnEnable()
        {
            _actionsCollectionEditor = new ReorderableListStackEditor<Actions_Action, ReorderableListComponentEditor>(
                new GUIContent("Actions"), Trigger.ActionCollection, true);

            _singleElementStackEditor =
                new SingleElementStackEditor<Events_Event, IMGUIElementEditor>(Trigger.SingleElementStack);
        }

        public override void OnInspectorGUI()
        {
            _singleElementStackEditor.OnGUI();

            GUILayout.Space(3);

            _actionsCollectionEditor.OnGUI();
        }
    }
}
#endif
