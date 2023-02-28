using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Counter
{
    public class CounterControl : VisualElement
    {
        public CounterControl()
        {
            VisualElement body = Resources.Load<VisualTreeAsset>($"View/Custom/Counter/counter").CloneTree();
            Add(body);
            textLabel = body.Q<Label>("text");
            valueLabel = body.Q<Label>("value");
            
            plusElement = body.Q("plus");
            minusElement = body.Q("minus");
            
            plusElement.RegisterCallback<ClickEvent>(evt => ValueInt++);
            minusElement.RegisterCallback<ClickEvent>(evt => ValueInt--);
            
            // Register both left and right clicks
            body.RegisterCallback<PointerDownEvent>(evt =>
            {
                body.CapturePointer(evt.pointerId);
            });
            body.RegisterCallback<PointerUpEvent>(evt =>
            {
                body.ReleasePointer(evt.pointerId);
                if (body.ContainsPoint(evt.localPosition))
                {
                    int mult = 1;
                    int val = evt.button == 0 ? 1 : -1;
                    if (Input.GetKey(KeyCode.LeftControl))
                        mult = 10;
                    ValueInt += val * mult;
                }
            });
        }
        
        public new class UxmlFactory : UxmlFactory<CounterControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription textInput = new() { name = "text-string", defaultValue = "serial" };

            private readonly UxmlIntAttributeDescription valueInput = new() { name = "value-int", defaultValue = 10 },
                minInput = new() { name = "min-value", defaultValue = 1 };
            private readonly UxmlIntAttributeDescription maxInput = new() { name = "max-value", defaultValue = 10000 };

            private readonly UxmlBoolAttributeDescription buttonsInput = new() {name = "buttons-enabled", defaultValue = true};
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as CounterControl;

                ate.TextString = textInput.GetValueFromBag(bag, cc);
                ate.ValueInt = valueInput.GetValueFromBag(bag, cc);
                ate.ButtonsEnabled = buttonsInput.GetValueFromBag(bag, cc);

                ate.MinValue = minInput.GetValueFromBag(bag, cc);
                ate.MaxValue = maxInput.GetValueFromBag(bag, cc);
            }
        }

        private VisualElement plusElement, minusElement;
        private Label textLabel, valueLabel;


        private string textString;
        private int valueInt;
        private bool buttonsEnabled;

        private int maxValue = 100000, minValue = 1;
        public string TextString
        {   
            get => textString;
            set
            {
                textString = value;
                textLabel.text = value;
            }
        }

        public int ValueInt
        {
            get => valueInt;
            set
            {
                valueInt = Mathf.Clamp(value, MinValue, MaxValue);
                valueLabel.text = valueInt.ToString();
            }
        }

        public bool ButtonsEnabled
        {
            get => buttonsEnabled;
            set
            {
                buttonsEnabled = value;
                valueLabel.text = valueInt.ToString();
                plusElement.style.display = value ? DisplayStyle.Flex :DisplayStyle.None;
                minusElement.style.display = value ? DisplayStyle.Flex :DisplayStyle.None;
            }        
        }

        public int MinValue
        {
            get => minValue;
            set => minValue = value;
        }
        public int MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }


    }


}
