using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Inputs
{
    public class Group(List<IElement> children) : IElement
    {
        private const float MARGIN = 6f;
        private const float PADDING = 10f;

        private OpRect rect = null;
        public readonly List<IElement> children = children;

        public float Height => children.Sum(x => x is ISpecialGroupHeight y ? y.GroupHeight : x.Height) + MARGIN * Math.Max(0, children.Count - 1) + 2 * PADDING;

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            float width = 600f - (x - 10f) / 10f * 6f - 20f;
            rect = new OpRect(new Vector2(x, y - Height), new Vector2(width, Height));
            if (_colorEdge.HasValue) rect.colorEdge = _colorEdge.Value;
            if (_colorFill.HasValue) rect.colorFill = _colorFill.Value;

            y -= PADDING + MARGIN;
            foreach (var child in children)
            {
                y -= MARGIN;
                child.Create(x + PADDING, ref y, elements);
            }

            y -= PADDING;
        }

        private Color? _colorEdge = null;
        public Color ColorEdge
        {
            set
            {
                _colorEdge = value;
                if (rect != null)
                {
                    rect.colorEdge = value;
                }
            }
        }

        private Color? _colorFill = null;
        public Color ColorFill
        {
            set
            {
                _colorFill = value;
                if (rect != null)
                {
                    rect.colorFill = value;
                }
            }
        }
    }
}
