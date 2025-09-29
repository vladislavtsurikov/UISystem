using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration.Utility;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UIComponentBinder
    {
        private readonly DiContainer _container;

        private readonly List<BoundComponentRecord> _records = new();
        private readonly UIBindingRepeatTracker _repeatTracker = new();
        private readonly UIHandler _uiHandler;

        public UIComponentBinder(DiContainer container, UIHandler handler)
        {
            _container = container;
            _uiHandler = handler;
        }

        public void BindUIComponentsFrom(GameObject instance)
        {
            IBindableUIComponent[] allComponents = instance.GetComponentsInChildren<IBindableUIComponent>(true);

            foreach (IBindableUIComponent component in allComponents)
            {
                Type type = component.GetType();
                var rawBindingId = component.BindingId;

                var index = _repeatTracker.GetAndIncrement(type, rawBindingId);

                var finalId = UIBindingId.FromTypeAndIndex(_uiHandler.GetType(), rawBindingId, index);

                _container
                    .Bind(type)
                    .WithId(finalId)
                    .FromInstance(component)
                    .AsCached();

                _records.Add(new BoundComponentRecord(type, finalId));
            }
        }

        public void Dispose()
        {
            foreach (BoundComponentRecord record in _records)
            {
                _container.UnbindId(record.Type, record.Id);
            }

            _records.Clear();
            _repeatTracker.Reset();
        }
    }
}
