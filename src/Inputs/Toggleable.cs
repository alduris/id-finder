using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Toggles an element's visibility with a yes/no button, separately from an enable checkbox.
    /// </summary>
    /// <typeparam name="E"></typeparam>
    public class Toggleable<E> : IElement, ISaveInHistory where E : IElement
    {
        /// <summary>Label used in switch input</summary>
        protected readonly string name;
        /// <summary>Input used to toggle element</summary>
        public readonly BoolInput ToggleInput;
        /// <summary>Element. Only actually displays when yes.</summary>
        public readonly E Element;

        /// <summary>Whether or not input is toggled to yes.</summary>
        public bool Toggled => ToggleInput.value;

        /// <summary>
        /// Creates an input that can be toggled using a yes/no button, separately from an enable checkbox.
        /// </summary>
        /// <param name="name">Name of toggle input</param>
        /// <param name="toggled">Default toggle state</param>
        /// <param name="element">Element to toggle</param>
        public Toggleable(string name, bool toggled, E element)
        {
            this.name = name;
            ToggleInput = new BoolInput(name, toggled) { hasBias = true };
            Element = element;

            ToggleInput.OnValueChanged += (_, v, o) => { if (v != o) SearchTab.instance.UpdateQueryBox(); };
        }

        /// <summary>Whether to force the toggle input to be force enabled</summary>
        public bool ForceEnabled
        {
            get => ToggleInput.forceEnabled;
            set => ToggleInput.forceEnabled = value;
        }
        /// <summary>Whether the toggle input itself is enabled</summary>
        public bool Enabled => ToggleInput.enabled;
        /// <summary>Whether the toggle input has bias</summary>
        public bool HasBias
        {
            get => ToggleInput.hasBias;
            set => ToggleInput.hasBias = value;
        }
        /// <summary>The bias of the toggle input</summary>
        public int Bias => ToggleInput.bias;

        /// <summary>Total height of the element.</summary>
        public float Height => ToggleInput.Height + (Toggled ? Element.Height + 6f : 0);

        /// <summary>Save key</summary>
        public string SaveKey => name;

        /// <summary>
        /// Creates the element
        /// </summary>
        /// <param name="x">What x to start from</param>
        /// <param name="y">What y to start from</param>
        /// <param name="elements">The list of elements to add to</param>
        public void Create(float x, ref float y, List<UIelement> elements)
        {
            ToggleInput.Create(x, ref y, elements);
            if (Toggled)
            {
                y -= 6f;
                Element.Create(x, ref y, elements);
            }
        }

        /// <summary>Converts the input into a format convertable to JSON via Newtonsoft.</summary>
        /// <returns>A struct containing the data for easy conversion</returns>
        public JObject ToSaveData()
        {
            return new JObject
            {
                ["toggle"] = ToggleInput.ToSaveData(),
                ["element"] = (Element is ISaveInHistory saveable) ? saveable.ToSaveData() : null,
            };
        }

        /// <summary>Returns the input to the state it was given the save state data.</summary>
        /// <param name="data">The save data to restore data to</param>
        public void FromSaveData(JObject data)
        {
            ToggleInput.FromSaveData((JObject)data["toggle"]!);
            if (Element is ISaveInHistory saveable)
            {
                saveable.FromSaveData((JObject)data["element"]!);
            }
        }

        /// <summary>Returns the string representation for the input on the history tab.</summary>
        /// <returns>The strings to represent the input on the history tab.</returns>
        public IEnumerable<string> GetHistoryLines()
        {
            foreach (var item in ToggleInput.GetHistoryLines())
            {
                yield return item;
            }
            if (Element is ISaveInHistory history)
            {
                foreach (var item in history.GetHistoryLines())
                {
                    yield return item;
                }
            }
        }
    }
}
