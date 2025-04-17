using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Menu.Remix.MixedUI;
using Newtonsoft.Json.Linq;

namespace FinderMod.Inputs
{
    public class Toggleable<E> : IElement, ISaveInHistory where E : IElement
    {
        protected readonly string name;
        public readonly BoolInput ToggleInput;
        public readonly E Element;

        public bool Toggled
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ToggleInput.value;
        }

        public Toggleable(string name, bool toggled, E element)
        {
            this.name = name;
            ToggleInput = new BoolInput(name, toggled) { hasBias = true };
            Element = element;
        }

        public float Height => ToggleInput.Height + (Toggled ? Element.Height + 6f : 0);

        public string SaveKey => name;

        public void Create(float x, ref float y, List<UIelement> elements)
        {
            ToggleInput.Create(x, ref y, elements);
            if (Toggled)
            {
                y -= 6f;
                Element.Create(x, ref y, elements);
            }
        }

        public JObject ToSaveData()
        {
            return new JObject
            {
                ["toggle"] = ToggleInput.ToSaveData(),
                ["element"] = (Element is ISaveInHistory saveable) ? saveable.ToSaveData() : null,
            };
        }

        public void FromSaveData(JObject data)
        {
            ToggleInput.FromSaveData((JObject)data["toggle"]!);
            if (Element is ISaveInHistory saveable)
            {
                saveable.FromSaveData((JObject)data["element"]!);
            }
        }

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
