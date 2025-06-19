using System;
using System.Collections.Generic;
using System.Linq;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Container for a group of <see cref="IElement"/>s.
    /// </summary>
    /// <param name="children">The list of children to add inside. Can also be set in the constructor if necessary with <see cref="children"/> field.</param>
    /// <param name="internalName">Internal name for group. Ideally should be unique. Used for saving in history.</param>
    public class Group(List<IElement> children, string internalName) : IElement, ISaveInHistory
    {
        /// <summary>Internal margin between elements</summary>
        protected const float MARGIN = 6f;
        /// <summary>Internal padding between edges</summary>
        protected const float PADDING = 10f;

        /// <summary>Internal name used in save data</summary>
        protected readonly string internalName = internalName;
        /// <summary>Outer edge. Null if not present.</summary>
        protected OpRect rect = null!;
        /// <summary>Whether element has an outer edge.</summary>
        public bool hasRect = true;
        /// <summary>List of children</summary>
        public List<IElement> children = children;

        /// <summary>Total height of the element.</summary>
        public float Height => children.Sum(x => x.Height) + MARGIN * Math.Max(0, children.Count - 1) + 2 * PADDING;

        /// <summary>
        /// Places the group and its children, optionally with the box if <see cref="hasRect"/> is set to <c>true</c> (default behavior).
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public virtual void Create(float x, ref float y, List<UIelement> elements)
        {
            if (hasRect)
            {
                float width = 600f - 20f - 20f - Mathf.Floor(x / 10f) * 20f;
                rect = new OpRect(new Vector2(x, y - Height), new Vector2(width, Height));
                if (_colorEdge.HasValue) rect.colorEdge = _colorEdge.Value;
                if (_colorFill.HasValue) rect.colorFill = _colorFill.Value;
                elements.Add(rect);
            }

            y -= PADDING - MARGIN;
            foreach (var child in children)
            {
                if (child == this) continue;
                y -= MARGIN;
                child.Create(x + PADDING, ref y, elements);
            }

            y -= PADDING;
        }

        private Color? _colorEdge = null;
        /// <summary>
        /// Sets the edge color of the box, if present.
        /// </summary>
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
        /// <summary>
        /// Sets the background color of the box, if present.
        /// </summary>
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


        /// <summary>Save key</summary>
        public string SaveKey => internalName;

        /// <summary>Converts the element into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public virtual JObject ToSaveData()
        {
            var list = new JObject();
            foreach (var child in children)
            {
                if (child is ISaveInHistory saveable)
                {
                    list.Add(saveable.SaveKey, saveable.ToSaveData());
                }
            }
            return list;
        }

        /// <summary>
        /// Returns the element to the state it was given the save state data.
        /// </summary>
        /// <param name="data">The save data to restore data to</param>
        public virtual void FromSaveData(JObject data)
        {
            HashSet<ISaveInHistory> saveable = children.Where(x => x is ISaveInHistory).Cast<ISaveInHistory>().ToHashSet();
            foreach (var kvp in data)
            {
                var child = saveable.FirstOrDefault(x => x.SaveKey == kvp.Key);
                if (child != null)
                {
                    child.FromSaveData((kvp.Value as JObject)!);
                    saveable.Remove(child);
                }
            }
        }

        /// <summary>Returns the string representation for the group on the history tab.</summary>
        /// <returns>The strings to represent the group on the history tab.</returns>
        public virtual IEnumerable<string> GetHistoryLines()
        {
            foreach (var child in children)
            {
                if (child is ISaveInHistory saveable)
                {
                    foreach (var line in saveable.GetHistoryLines())
                    {
                        yield return line;
                    }
                }
            }
            yield break;
        }
    }
}
