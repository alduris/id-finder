using System;
using System.Collections.Generic;
using FinderMod.Tabs;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    /// <summary>
    /// Alternates between two elements
    /// </summary>
    /// <typeparam name="E">Element type to alternate between</typeparam>
    public class Alternatable<E> : IElement, ISaveInHistory where E : IElement
    {
        protected readonly string name;
        protected BoolInput SwitchInput;
        protected readonly E elementA;
        protected readonly E elementB;

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

        public bool ForceEnabled
        {
            set => SwitchInput.forceEnabled = value;
        }
        public bool Enabled => SwitchInput.enabled;
        public bool HasBias
        {
            get => SwitchInput.hasBias;
            set => SwitchInput.hasBias = value;
        }
        public int Bias => SwitchInput.bias;
        public bool Switch => SwitchInput.value;
        public E Element => Enabled ? (SwitchInput.value ? elementB : elementA) : default!;
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
