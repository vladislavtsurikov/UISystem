#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Editor.Core.SelectionDatas.Group.TemplatesSystem
{
    public class TemplateStackEditor
    {
        private readonly List<Template> _templateList;

        public TemplateStackEditor()
        {
            _templateList = new List<Template>();

            foreach (Type type in AllTemplateTypes.TypeList)
            {
                Create(type);
            }
        }

        public void ShowMenu(GenericMenu menu, Type toolType, Runtime.Core.SelectionDatas.Group.Group group,
            SelectedData selectedData)
        {
            if (!TemplateUtility.HasTemplate(toolType, group.PrototypeType))
            {
                return;
            }

            menu.AddSeparator("");

            for (var i = 0; i < AllTemplateTypes.TypeList.Count; i++)
            {
                TemplateAttribute templateAttribute = AllTemplateTypes.TypeList[i].GetAttribute<TemplateAttribute>();

                var name = templateAttribute.Name;
                Type[] supportedResourceTypes = templateAttribute.SupportedResourceTypes;

                Template template = _templateList[i];

                if (templateAttribute.ToolTypes.Contains(toolType))
                {
                    menu.AddItem(new GUIContent("Apply Templates/" + name), false,
                        ContextMenuUtility.ContextMenuCallback, new Action(() =>
                            selectedData.SelectedPrototypeList.ForEach(proto =>
                            {
                                template.Apply(supportedResourceTypes, group, proto);
                            })));
                }
            }
        }

        private void Create(Type type)
        {
            var editor = (Template)Activator.CreateInstance(type);
            _templateList.Add(editor);
        }
    }
}
#endif
