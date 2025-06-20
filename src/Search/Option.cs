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
    public abstract partial class Option()
    {
        internal string name = "Search Option";

        internal bool firstOption = false;
        internal bool linked = false;

        /// <summary>
        /// The elements to show in the space. Must be added to in the constructor.
        /// </summary>
        protected List<IElement> elements = null!;

        /// <summary>
        /// Creates the UI elements for the Remix menu. Called internally.
        /// </summary>
        /// <param name="y">A variable initialized to the y-value of the top of the option</param>
        /// <param name="output">The list of UI elements to output to, for adding to the remix menu later.</param>
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
        /// Execution, assuming an arbitrary given seed. Returns the distance of the calculated fields for the option from the values in the inputs.
        /// <para>Things to keep in mind for a good distance calculation:</para>
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// All inputs that should affect the result should be accounted for.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Most common input types are accounted for with the built-in <c>DistanceIf</c> helper methods, which already account for most factors.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If an input does not have a <c>DistanceIf</c> implementation, it is up to you to replace its calculation.
        /// An ideal distance calculation should be between 0 and 1, then multiplied by the input's bias.
        /// If the input is disabled, it should contribute 0 distance.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="Random">The random number generator correlating with the seed. Use like <see cref="UnityEngine.Random"/>.</param>
        /// <returns>The calculated distance.</returns>
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

        /// <summary>
        /// Returns strings to display in the Values tab for a particular id.
        /// It is recommended to use <c>yield return</c> when implementing this method.
        /// <c>null</c> is treated as whitespace, and is a shorter amount than there would be by returning an empty string.
        /// </summary>
        /// <param name="Random">An instance of <see cref="XORShift128"/>, initialized to the seed of the creature.</param>
        /// <returns>A string representation of the traits for an id</returns>
        protected abstract IEnumerable<string> GetValues(XORShift128 Random);

        /// <summary>
        /// Force updates the query box in the Search tab.
        /// </summary>
        public static void UpdateQueryBox()
        {
            SearchTab.instance?.UpdateQueryBox();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Save stuff

        /// <summary>
        /// Converts the Option to a JSON representation, for saving or sharing.
        /// </summary>
        /// <returns>A JSON representation of the Option stored as a <see cref="JObject"/></returns>
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

        /// <summary>
        /// Returns the Option to the state stored in the JSON. Assumes JSON is properly formatted, and may throw an exception otherwise.
        /// </summary>
        /// <param name="json">A <see cref="JObject"/> representation containing the JSON of the object</param>
        public void FromJson(JObject json)
        {
            linked = (bool)json["linked"]!;
            HashSet<ISaveInHistory> saveable = [.. elements.Where(x => x is ISaveInHistory).Cast<ISaveInHistory>()];
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

        /// <summary>
        /// Returns a string representation of the current configuration of inputs in the Option. Used in the history tab.
        /// </summary>
        /// <returns>A string representation of the current configuruation of inputs in the Option</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var child in elements)
            {
                if (child is ISaveInHistory saveable)
                {
                    foreach (var line in saveable.GetHistoryLines())
                    {
                        if (line is null)
                            sb.AppendLine();
                        else
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
