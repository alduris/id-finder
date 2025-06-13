using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinderMod.Inputs;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static Menu.Menu;

namespace FinderMod.Search.Options
{
    /// <summary>
    /// A search option. Fill out <see cref="elements"/> in the constructor, and keep references so you can use the values in <see cref="Execute"/>.
    /// </summary>
    /// <param name="key">Serves as both the internal key the option will be identified as</param>
    public abstract partial class Option()
    {
        internal string name = "Search Option";

        internal bool firstOption = false;
        internal bool linked = false;

        /// <summary>
        /// The elements to show in the space. Must be added to in the constructor.
        /// </summary>
        protected List<IElement> elements = null!;

        public void CreateOptions(ref float y, List<UIelement> output)
        {
            const float MARGIN = 6f;

            if (!firstOption && !linked)
            {
                y -= 2f;
                output.Add(new OpImage(new Vector2(10f, y), "pixel") { scale = new Vector2(580f, 2f), color = MenuRGB(MenuColors.MediumGrey) });
                y -= MARGIN;
            }

            y -= 27f;
            var deleteButton = new OpSimpleButton(new Vector2(10f, y), new Vector2(24f, 24f), "\xd7") { colorEdge = OpUtil.RedColor, colorFill = OpUtil.RedColor };
            deleteButton.OnClick += (_) => OnDelete?.Invoke();
            var linkButton = new OpSimpleButton(new Vector2(40f, y), new Vector2(24f, 24f), linked ? "-" : "+") { colorEdge = OpUtil.GreenColor, colorFill = OpUtil.GreenColor };
            if (!firstOption)
            {
                linkButton.OnClick += (_) =>
                {
                    linked = !linked;
                    OnLink?.Invoke();
                };
            }
            else
            {
                linked = false;
                linkButton.greyedOut = true;
            }
            output.Add(deleteButton);
            output.Add(linkButton);

            y -= 3f;
            output.Add(new OpLabel(70f, y, name, true));

            foreach (var element in elements)
            {
                y -= MARGIN;
                element.Create(10f, ref y, output);
            }
        }

        /// <summary>
        /// Execution, assuming an arbitrary given seed.
        /// </summary>
        /// <param name="Random">The random number generator correlating with the seed.</param>
        /// <returns>The calculated distance</returns>
        public abstract float Execute(XORShift128 Random);

        /// <summary>
        /// Values to display in the the values tab.
        /// </summary>
        /// <param name="id">The id of the seed. Feed into</param>
        /// <returns>Strings to display in labels, or null for whitespace.</returns>
        public IEnumerable<string> GetValues(int id)
        {
            var random = new XORShift128();
            random.InitState(id);
            foreach (var str in GetValues(random))
            {
                yield return str;
            }
        }
        protected abstract IEnumerable<string> GetValues(XORShift128 Random);

        internal protected static void UpdateQueryBox()
        {
            SearchTab.instance?.UpdateQueryBox();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Save stuff
        public JObject ToJson()
        {
            var children = new JObject();
            foreach (var child in elements)
            {
                if (child is ISaveInHistory saveable)
                {
                    children[saveable.SaveKey] = saveable.ToSaveData();
                }
            }
            return new JObject()
            {
                ["name"] = name,
                ["linked"] = linked,
                ["children"] = children
            };
        }

        public void FromJson(JObject json)
        {
            linked = (bool)json["linked"]!;
            HashSet<ISaveInHistory> saveable = elements.Where(x => x is ISaveInHistory).Cast<ISaveInHistory>().ToHashSet();
            foreach (var kvp in (JObject)json["children"]!)
            {
                var child = saveable.FirstOrDefault(x => x.SaveKey == kvp.Key);
                if (child != null)
                {
                    child.FromSaveData((kvp.Value as JObject)!);
                    saveable.Remove(child);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var child in elements)
            {
                if (child is ISaveInHistory saveable)
                {
                    foreach (var line in saveable.GetHistoryLines())
                    {
                        sb.AppendLine(line.ToString());
                    }
                }
            }
            return sb.ToString();
        }

        // Internal events
        internal event Action? OnDelete;
        internal event Action? OnLink;
    }
}
