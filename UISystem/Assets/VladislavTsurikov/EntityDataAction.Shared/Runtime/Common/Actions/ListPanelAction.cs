using System.Collections.Generic;
using VladislavTsurikov.EntityDataAction.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Common
{
    public abstract class ListPanelAction<TItem, TView> : EntityAction where TView : Entity
    {
        protected abstract List<TView> Views { get; }

        protected void ApplyList(IReadOnlyList<TItem> items)
        {
            if (Views == null)
            {
                return;
            }

            int viewsCount = Views.Count;
            int itemsCount = items.Count;
            int limit = viewsCount < itemsCount ? viewsCount : itemsCount;

            for (int i = 0; i < limit; i++)
            {
                TView view = Views[i];
                if (view == null)
                {
                    continue;
                }

                TItem item = items[i];
                ShowItem(view, item, i);
            }

            DisableUnusedViews(limit, viewsCount);
        }

        protected abstract void ShowItem(TView view, TItem item, int index);

        protected void DisableAllViews()
        {
            if (Views == null)
            {
                return;
            }

            DisableUnusedViews(0, Views.Count);
        }

        protected void DisableUnusedViews(int startIndex, int endIndex)
        {
            if (Views == null)
            {
                return;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                TView view = Views[i];
                if (view != null)
                {
                    view.gameObject.SetActive(false);
                }
            }
        }
    }
}
