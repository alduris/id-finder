using Menu;
using Menu.Remix;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;

namespace FinderMod.Tabs
{
    internal abstract class BaseTab : Page
    {
        private readonly MenuTabWrapper _tabWrapper;
        protected readonly Vector2 basePos;
        protected readonly Vector2 baseSize;

        public SelectableMenuObject? defaultSelectable = null;
        
        protected FinderProcess FinderMenu => (menu as FinderProcess)!;
        
        protected BaseTab(FinderProcess menu, string name, int pageIndex) : base(menu, null, name, pageIndex)
        {
            _tabWrapper = new MenuTabWrapper(menu, this);
            subObjects.Add(_tabWrapper);

            const float HORIZ_EDGE_PAD = 20f;
            const float VERT_EDGE_PAD = 50f; // 20f normal + 30f for labels
            var smallestScreenSize = new Vector2(1024f, 768f);
            basePos = new Vector2(1366f / 2f - smallestScreenSize.x / 2f + HORIZ_EDGE_PAD, VERT_EDGE_PAD);
            baseSize = new Vector2(smallestScreenSize.x - 2 * HORIZ_EDGE_PAD, smallestScreenSize.y - 2 * VERT_EDGE_PAD - 45f);
        }
        
        /// <summary>
        /// Update method only called when this tab is the active tab
        /// </summary>
        protected abstract void InternalUpdate();

        public override void Update()
        {
            base.Update();
            if (FinderMenu.ActiveTab == index)
            {
                InternalUpdate();
            }
        }

        public virtual void ClearMemory() { }

        protected UIelementWrapper? WrapperFor(UIelement? item)
        {
            if (item is null) return null;
            return _tabWrapper.wrappers.TryGetValue(item, out UIelementWrapper wrapper) ? wrapper : null;
        }

        protected bool TryGetWrapperFor(UIelement? item, out UIelementWrapper? wrapper)
        {
            wrapper = WrapperFor(item);
            return wrapper is not null;
        }


        private void _AddItem(UIelement item)
        {
            if (!_tabWrapper.wrappers.ContainsKey(item))
            {
                _ = new UIelementWrapper(_tabWrapper, item);
            }
        }

        protected void AddItems(params UIelement?[] items)
        {
            foreach (UIelement? item in items)
            {
                if (item is not null)
                {
                    _AddItem(item);
                }
            }
        }

        private void _RemoveItem(UIelement item)
        {
            if (_tabWrapper.wrappers.TryGetValue(item, out UIelementWrapper wrapper))
            {
                _tabWrapper._tab._RemoveItem(item);
                _tabWrapper.RemoveSubObject(wrapper);
                _tabWrapper.wrappers.Remove(item);
            }
        }

        protected void RemoveItems(params UIelement?[] items)
        {
            foreach (UIelement? item in items)
            {
                if (item is not null)
                {
                    _RemoveItem(item);
                }
            }
        }

        protected void MutualHorizontalButtonBind(UIfocusable left, UIfocusable right)
        {
            if (_tabWrapper.wrappers.TryGetValue(left, out UIelementWrapper leftWrapper) &&
                _tabWrapper.wrappers.TryGetValue(right, out UIelementWrapper rightWrapper))
            {
                menu.MutualHorizontalButtonBind(leftWrapper, rightWrapper);
            }
        }

        protected void MutualVerticalButtonBind(UIfocusable top, UIfocusable bottom)
        {
            if (_tabWrapper.wrappers.TryGetValue(top, out UIelementWrapper topWrapper) &&
                _tabWrapper.wrappers.TryGetValue(bottom, out UIelementWrapper bottomWrapper))
            {
                menu.MutualVerticalButtonBind(bottomWrapper, topWrapper);
            }
        }

        public static string Translate(string text) => Custom.rainWorld.inGameTranslator.TryTranslate(text, out string translated) ? translated : text;
    }
}
