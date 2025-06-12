using System;
using System.Collections.Generic;
using FinderMod.Preview;
using Menu.Remix.MixedUI;
using RWCustom;
using UnityEngine;
using static FinderMod.OpUtil;

namespace FinderMod.Tabs
{
    internal class PreviewTab(OptionInterface option) : BaseTab(option, "Preview")
    {
        private OpComboBox creatureInput = null!;
        private OpTextBox idInput = null!;
        private OpGamePreview gameWindow = null!;

        public override void Initialize()
        {
            List<string> creatures = [.. CreatureTemplate.Type.values.entries];
            creatures.Sort();
            creatureInput = new OpComboBox(CosmeticBind(""), new Vector2(), 250f, creatures.ToArray()) { listHeight = 24 };
            idInput = new OpTextBox(CosmeticBind(0), new(10f + creatureInput.size.x + 40f, creatureInput.pos.y), 100f) { allowSpace = true };
            gameWindow = new OpGamePreview(new Vector2(10f, 10f), new Vector2(580f, 400f));

            AddItems(
                new OpLabel(10f, 560f, "Preview", true),
                new OpLabel(10f + creatureInput.size.x + 20f, creatureInput.pos.y, "ID:"),
                idInput,
                creatureInput,
                gameWindow
            );

            creatureInput.OnValueChanged += OnValueChange;
            idInput.OnValueUpdate += OnValueChange;

            gameWindow.Initialize(Custom.rainWorld.processManager);

            void OnValueChange(UIconfig _, string value, string oldValue)
            {
                if (value != oldValue && creatureInput.value != null && int.TryParse(idInput.value, out int id)) SpawnCritter(new CreatureTemplate.Type(creatureInput.value), id);
            }
        }

        public override void Update()
        {
        }

        public override void ClearMemory()
        {
            base.ClearMemory();
            PreviewManager.Uninitialize();
        }

        internal void UpdateInputs(CreatureTemplate.Type type, int id)
        {
            creatureInput.value = type.ToString();
            idInput.valueInt = id;
            SpawnCritter(type, id);
        }

        public void SpawnCritter(CreatureTemplate.Type type, int id)
        {
            if (type == null || type.Index < 0) return;

            try
            {
                gameWindow.SpawnCreature(type, id);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                Plugin.logger.LogError(ex);
            }
        }
    }
}
