using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Level
{
    public class LevelControl : VisualElement
    {

        public new class UxmlFactory : UxmlFactory<LevelControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            

            private readonly UxmlStringAttributeDescription levelInput = new() { name = "level", defaultValue = "10" };
            private readonly UxmlColorAttributeDescription backgroundColorInput = new() { name = "background-color", defaultValue = Color.HSVToRGB(0,0,.45f)};
            private readonly UxmlEnumAttributeDescription<LevelSizeEnum> levelSizeInput = new()
                { name = "level-size", defaultValue = LevelSizeEnum.Medium };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as LevelControl;

                ate.Level = levelInput.GetValueFromBag(bag, cc);
                ate.BackgroundColor = backgroundColorInput.GetValueFromBag(bag, cc);
                ate.LevelSize = levelSizeInput.GetValueFromBag(bag, cc);
            }
        }

        private Label label;
        
        // Level
        private string level;
        public string Level
        {
            get => level;
            set
            {
                level = value;
                label.text = value;
            }
        }

        // Color
        private Color backgroundColor;
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                label.style.backgroundColor = value;
            } 
        }



        // Size
        public enum LevelSizeEnum
        {
            Small,
            Medium, 
            Big,
            Large,
            Extra
        }

        private LevelSizeEnum levelSize;
        
        public LevelSizeEnum LevelSize
        {
            get => levelSize;
            set
            {
                levelSize = value;
                ApplySizeStyle(GetSizeStyle(value));
            } 
        }

#pragma warning disable CS8524
        SizeStyle GetSizeStyle(LevelSizeEnum levelSizeEnum) => levelSizeEnum switch
#pragma warning restore CS8524
        {
            LevelSizeEnum.Small => new SizeStyle(10, 20, 2),
            LevelSizeEnum.Medium => new SizeStyle(12, 24, 2),
            LevelSizeEnum.Big => new SizeStyle(16, 32, 3),
            LevelSizeEnum.Large => new SizeStyle(20, 42, 4),
            LevelSizeEnum.Extra => new SizeStyle(12, 30,2)
        };
        void ApplySizeStyle(SizeStyle sizeStyle)
        {
            label.style.fontSize = sizeStyle.FontSize;
            label.style.width = sizeStyle.Size;
            label.style.height = sizeStyle.Size;
            label.style.borderBottomWidth = sizeStyle.BorderWidth;
            label.style.borderLeftWidth = sizeStyle.BorderWidth;
            label.style.borderRightWidth = sizeStyle.BorderWidth;
            label.style.borderTopWidth = sizeStyle.BorderWidth;
        }

        public LevelControl()
        {
            VisualElement body = Resources.Load<VisualTreeAsset>($"View/Custom/Level/level").CloneTree();
            Add(body);
            label = body.Q<Label>();
        }
    }

    public struct SizeStyle
    {
        public readonly int FontSize, Size, BorderWidth;

        public SizeStyle(int fontSize, int size, int borderWidth)
        {
            FontSize = fontSize;
            Size = size;
            BorderWidth = borderWidth;
        }
    }
}
