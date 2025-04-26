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
        protected readonly string name;
        protected MultiChoiceInput SwitchInput;
        protected List<E> elements;

        /// <param name="name">Name of the switch input</param>
        /// <param name="options">Elements to switch between, key-element pairs.</param>
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

        public bool Enabled => SwitchInput.enabled;
        public bool HasBias
        {
            get => SwitchInput.hasBias;
            set => SwitchInput.hasBias = value;
        }
        public int Bias => SwitchInput.bias;
        public int Switch => SwitchInput.value;
        public E Element => Enabled ? elements[SwitchInput.value] : default!;
        public float Height => SwitchInput.Height;

        public string SaveKey => name;

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            SwitchInput.Create(x, ref y, elements);
            if (Enabled)
            {
                y -= 6f;
                Element.Create(x, ref y, elements);
            }
        }

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
