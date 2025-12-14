#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.AddressableLoaderSystem.Editor.Core
{
    public abstract class ResourceLoaderTemplate
    {
        private string _className;

        public string ClassName
        {
            get
            {
                if (LoaderType != null)
                {
                    return LoaderType.Name;
                }

                return _className;
            }
            set
            {
                if (LoaderType == null)
                {
                    _className = value;
                }
            }
        }

        public Type LoaderType { get; private set; }
        public string CsFilePath { get; private set; }

        public List<BehaviorAttributeData> Behaviors = new();

        public abstract void Run();

        protected virtual void OnBuildFrom(Type loaderType)
        {
        }

        public void BuildFrom(Type loaderType)
        {
            LoaderType = loaderType;
            CsFilePath = loaderType.GetSourceFilePath();
            _className = loaderType.Name;

            OnBuildFrom(loaderType);
        }

        public string GetBaseTypeName()
        {
            return GetType().GetAttribute<ResourceLoaderTemplateBaseTypeAttribute>().Type.Name;
        }

        public virtual void Validate(List<string> issues)
        {
        }
    }
}
#endif
