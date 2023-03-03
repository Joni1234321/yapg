using Bserg.View.Custom.Progress;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Level
{
    public class LevelGroupControl : VisualElement
    {

        public new class UxmlFactory : UxmlFactory<LevelGroupControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription textInput = new() { name = "text", defaultValue = "something" };
            private readonly UxmlStringAttributeDescription levelInput = new() { name = "level", defaultValue = "X" };
            private readonly UxmlBoolAttributeDescription progressEnabledInput = new() { name = "progress-enabled", defaultValue = false };
            private readonly UxmlBoolAttributeDescription reverseInput = new() { name = "reverse", defaultValue = false };
            private readonly UxmlFloatAttributeDescription valueInput = new() { name = "value", defaultValue = 31.2f };
            private readonly UxmlColorAttributeDescription backgroundColorInput = new() { name = "background-color", defaultValue = Color.HSVToRGB(0,0,.45f)};
            private readonly UxmlEnumAttributeDescription<LevelControl.LevelSizeEnum> levelSizeInput = new()
                { name = "level-size", defaultValue = LevelControl.LevelSizeEnum.Big };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as LevelGroupControl;
                
                ate.Text = textInput.GetValueFromBag(bag, cc);
                ate.Level = levelInput.GetValueFromBag(bag, cc);
                ate.Value = valueInput.GetValueFromBag(bag, cc);
                ate.ProgressEnabled = progressEnabledInput.GetValueFromBag(bag, cc);
                ate.Reverse = reverseInput.GetValueFromBag(bag, cc);
                ate.BackgroundColor = backgroundColorInput.GetValueFromBag(bag, cc);
                ate.LevelSize = levelSizeInput.GetValueFromBag(bag, cc);
            }
        }

        private VisualElement body;
        private Label nameLabel;
        private LevelControl levelControl;
        private ProgressControl progressControl;
        
        // Level
        public string Level
        {
            get => levelControl.Level;
            set => levelControl.Level = value;
        }

        public float Value
        {
            get => progressControl.Value;
            set => progressControl.Value = value;
        }

        private bool progressEnabled, reverse;

        public bool ProgressEnabled
        {
            get => progressEnabled;
            set
            {
                progressEnabled = value;
                progressControl.style.display = value ? DisplayStyle.Flex :DisplayStyle.None;
            }
        }
        
        public bool Reverse
        {
            get => reverse;
            set
            {
                reverse = value;
                body.style.flexDirection = value ? FlexDirection.Row : FlexDirection.RowReverse;
            }
        }

        private string text;
        public string Text
        {
            get => text;
            set
            {
                text = value;
                nameLabel.text = value;
            }
        }

        // Color
        public Color BackgroundColor
        {
            get => levelControl.BackgroundColor;
            set
            {
                levelControl.BackgroundColor = value; 
                nameLabel.style.borderBottomColor = value;
                progressControl.Fill = value;
            }

        }



        // Size
        public LevelControl.LevelSizeEnum LevelSize
        {
            get => levelControl.LevelSize;
            set => levelControl.LevelSize = value;
        }

        public LevelGroupControl()
        {
            VisualElement root = Resources.Load<VisualTreeAsset>($"View/Custom/Level/level-group").CloneTree();
            body = root.Q("level-group");
            Add(root);
            levelControl = root.Q<LevelControl>();
            nameLabel = root.Q<Label>("name");
            progressControl = root.Q<ProgressControl>();
            
        }
        
     
    }

}
