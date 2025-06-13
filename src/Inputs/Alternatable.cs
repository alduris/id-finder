using System;
using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Alternates between two elements using a Yes/No button.
    /// </summary>
    /// <typeparam name="E">Element type to alternate between</typeparam>
    public class Alternatable<E> : IElement, ISaveInHistory where E : IElement
    {
        /// <summary>Label used in switch input</summary>
        protected readonly string name;
        /// <summary>Input used to switch between two elements</summary>
        protected BoolInput SwitchInput;
        /// <summary>Element when no</summary>
        protected readonly E elementA;
        /// <summary>Element when yes</summary>
        protected readonly E elementB;

        /// <summary>
        /// Creates an input holder that switches between two inputs using a Yes/No button.
        /// </summary>
        /// <param name="name">Name to display on switch input</param>
        /// <param name="alternated">Default value of switch input</param>
        /// <param name="elementA">Element to show when "No"</param>
        /// <param name="elementB">Element to show when "Yes"</param>
        public Alternatable(string name, bool alternated, E elementA, E elementB)
        {
            this.name = name;
            SwitchInput = new(name, alternated);
            this.elementA = elementA;
            this.elementB = elementB;

            SwitchInput.OnValueChanged += (_, v, o) =>
            {
                if (v != o)
                {
                    SearchTab.instance.UpdateQueryBox();
                }
            };
        }

        /// <summary>Whether to force the switch input to be force enabled</summary>
        public bool ForceEnabled
        {
            get => SwitchInput.forceEnabled;
            set => SwitchInput.forceEnabled = value;
        }
        /// <summary>Whether the switch input itself is enabled</summary>
        public bool Enabled => SwitchInput.enabled;
        /// <summary>Whether the switch input has bias</summary>
        public bool HasBias
        {
            get => SwitchInput.hasBias;
            set => SwitchInput.hasBias = value;
        }
        /// <summary>The bias of the switch input</summary>
        public int Bias => SwitchInput.bias;
        /// <summary>Whether or not the switch input is set to "Yes"</summary>
        public bool Switch => SwitchInput.value;
        /// <summary>The active element according to the value of <see cref="Switch"/></summary>
        public E Element => Enabled ? (SwitchInput.value ? elementB : elementA) : default!;

        /// <summary>Total height of the element.</summary>
        public float Height => SwitchInput.Height + (Enabled ? Element.Height + 6f : 0f);

        /// <summary>Save Key</summary>
        public string SaveKey => name;

        /// <summary>
        /// Creates the element
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            SwitchInput.Create(x, ref y, elements);
            if (Enabled)
            {
                y -= 6f;
                Element.Create(x, ref y, elements);
            }
        }

        /// <summary>Converts the input into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public JObject ToSaveData()
        {
            var obj = new JObject()
            {
                ["switch"] = SwitchInput.ToSaveData(),
            };
            if (elementA is ISaveInHistory historyA)
            {
                obj["a"] = historyA.ToSaveData();
            }
            if (elementB is ISaveInHistory historyB)
            {
                obj["b"] = historyB.ToSaveData();
            }
            return obj;
        }

        /// <summary>Returns the input to the state it was given the save state data.</summary>
        /// <param name="data">The save data to restore data to</param>
        public void FromSaveData(JObject data)
        {
            SwitchInput.FromSaveData((JObject)data["switch"]!);
            if (elementA is ISaveInHistory historyA)
            {
                historyA.FromSaveData((JObject)data["a"]!);
            }
            if (elementB is ISaveInHistory historyB)
            {
                historyB.FromSaveData((JObject)data["b"]!);
            }
        }

        /// <summary>Returns the string representation for the input on the history tab.</summary>
        /// <returns>The strings to represent the input on the history tab.</returns>
        public IEnumerable<string> GetHistoryLines()
        {
            if (Enabled && Element is ISaveInHistory history)
            {
                foreach (var line in history.GetHistoryLines())
                {
                    yield return line;
                }
            }
        }
    }

    /// <summary>
    /// Version of <see cref="Alternatable{E}"/> without type argument. (Internally uses with <see cref="IElement"/>)
    /// </summary>
    /// <param name="name">Display name of the switcher</param>
    /// <param name="alternated">Default configuration</param>
    /// <param name="elementA">Element when not alternated</param>
    /// <param name="elementB">Element when alternated</param>
    public class Alternatable(string name, bool alternated, IElement elementA, IElement elementB) : Alternatable<IElement>(name, alternated, elementA, elementB)
    {
    }
}
