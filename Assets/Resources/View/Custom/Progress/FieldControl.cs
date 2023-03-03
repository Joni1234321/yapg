using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Progress
{
    public class ProgressControl : VisualElement
    {

        public new class UxmlFactory : UxmlFactory<ProgressControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlBoolAttributeDescription progressVertical = new() { name = "vertical", defaultValue = false};

            private readonly UxmlColorAttributeDescription progressColor = new() { name = "fill", defaultValue = Color.HSVToRGB(.64f, .4f, .4f)};
            private readonly UxmlFloatAttributeDescription progressValue = new() { name = "value", defaultValue = 50f };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as ProgressControl;

                ate.Vertical = progressVertical.GetValueFromBag(bag, cc);
                ate.Fill = progressColor.GetValueFromBag(bag, cc);
                ate.Value = progressValue.GetValueFromBag(bag, cc);
            }
        }

        private bool isVertical;
        private float valueProgress;
        private Color fill;
        private readonly VisualElement progress;
        

        public ProgressControl()
        {
            VisualElement body = Resources.Load<VisualTreeAsset>($"View/Custom/Progress/progress").CloneTree();
            Add(body);
            progress = body.Q<VisualElement>("progress");
            body.style.flexGrow = 1;
        }

        public Color Fill
        {
            get => fill;
            set
            {
                fill = value;
                progress.style.backgroundColor = value;
            } 
        }

        public float Value
        {
            get => valueProgress;
            set
            {
                this.valueProgress = value;
                if (Vertical)
                    progress.style.maxHeight = Length.Percent(value);
                else
                    progress.style.width = Length.Percent(value);
            } 
        }

        public bool Vertical
        {
            get => isVertical;
            set
            {
                isVertical = value;
                if (value)
                {
                    progress.style.width = Length.Percent(100);
                    progress.style.maxHeight = Length.Percent(Value);
                }
                else
                {
                    progress.style.width = Length.Percent(Value);
                    progress.style.maxHeight = Length.Percent(100); 
                }
            }
        }
    }
}
