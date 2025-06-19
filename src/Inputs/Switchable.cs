using System;
using System.Collections.Generic;
using System.Linq;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Switch between multiple elements based on a combo box.
    /// </summary>
    /// <typeparam name="E">The type of element to switch between</typeparam>
    public class Switchable<E> : IElement, ISaveInHistory where E : IElement
    {
        /// <summary>Label used in switch input</summary>
        protected readonly string name;
        /// <summary>Input used to switch between elements</summary>
        protected MultiChoiceInput SwitchInput;
        /// <summary>Possible elements</summary>
        protected List<E> elements;

        /// <summary>
        /// Creates an input holder that switches between elements based on the value of a combo box.
        /// </summary>
        /// <param name="name">Name to display on switch input</param>
        /// <param name="options">List of options using switch input label-resulting element tuples</param>
        public Switchable(string name, List<(string key, E element)> options)
        {
            this.name = name;
            SwitchInput = new(name, [.. options.Select(x => x.key)]);
            elements = [.. options.Select(x => x.element)];

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
        /// <summary>Selected index of the switch input</summary>
        public int Switch => SwitchInput.value;
        /// <summary>The active element according to the value of <see cref="Switch"/></summary>
        public E Element => Enabled ? elements[SwitchInput.value] : default!;

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
            var elData = new JArray();
            foreach (var element in elements)
            {
                if (element is ISaveInHistory history)
                {
                    elData.Add(history.ToSaveData());
                }
            }
            obj["options"] = elData;
            return obj;
        }

        /// <summary>Returns the input to the state it was given the save state data.</summary>
        /// <param name="data">The save data to restore data to</param>
        public void FromSaveData(JObject data)
        {
            SwitchInput.FromSaveData((JObject)data["switch"]!);
            for (int i = 0, j = 0; i < elements.Count; i++)
            {
                if (elements[i] is ISaveInHistory element)
                {
                    element.FromSaveData((JObject)((JArray)data["options"]!)[j]);
                    j++;
                }
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
    /// Version of <see cref="Switchable{E}"/> without the type argument. (Internally uses <see cref="IElement"/>)
    /// </summary>
    /// <param name="name">Name of the switcher</param>
    /// <param name="options">Elements to switch between, key-element pairs</param>
    public class Switchable(string name, List<(string key, IElement element)> options) : Switchable<IElement>(name, options)
    {
    }
}
