using System;
using System.Collections.Generic;
using FinderMod.Inputs;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace FinderMod.Search.Options
{
    public abstract class Option(string name)
    {
        private readonly string name = name;

        internal bool firstOption = false;

        /// <summary>
        /// The elements to show in the space. Must be added to in the constructor.
        /// </summary>
        protected readonly List<IElement> elements = [];

        public void CreateOptions(ref float y, List<UIelement> output)
        {
            const float MARGIN = 6f;

            if (!firstOption)
            {
                y -= 2f;
                output.Add(new OpImage(new Vector2(10f, y), "pixel") { scale = new Vector2(580f, 2f) });
                y -= MARGIN;
            }

            y -= 27f;
            var deleteButton = new OpSimpleButton(new Vector2(10f, y), new Vector2(24f, 24f), "\xd7") { colorEdge = OpUtil.color_del, colorFill = OpUtil.color_del };
            deleteButton.OnClick += (_) => OnDelete?.Invoke();
            var linkButton = new OpSimpleButton(new Vector2(40f, y), new Vector2(24f, 24f), "+") { colorEdge = OpUtil.color_link, colorFill = OpUtil.color_link };
            if (firstOption)
                linkButton.OnClick += (_) => OnLink?.Invoke();
            else
                linkButton.greyedOut = true;
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

        public abstract void Execute();

        internal event Action OnDelete;
        internal event Action OnLink;
    }
}
