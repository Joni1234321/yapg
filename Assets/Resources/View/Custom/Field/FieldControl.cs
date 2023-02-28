using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Field
{
    public class FieldControl : VisualElement
    {

        public new class UxmlFactory : UxmlFactory<FieldControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription fieldName = new() { name = "title", defaultValue = "Moneys" };
            private readonly UxmlStringAttributeDescription fieldValue = new() { name = "value", defaultValue = "100 K" };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as FieldControl;

                ate.Title = fieldName.GetValueFromBag(bag, cc);
                ate.Value = fieldValue.GetValueFromBag(bag, cc);
            }
        }

        private string nameString, valueString;
        private readonly Label nameLabel, valueLabel;
        

        public FieldControl()
        {
            VisualElement body = Resources.Load<VisualTreeAsset>($"View/Custom/Field/field").CloneTree();
            Add(body);
            nameLabel = body.Q<Label>("name");
            valueLabel = body.Q<Label>("value");
        }

        public string Title
        {
            get => nameString;
            set
            {
                nameString = value;
                nameLabel.text = value;
            } 
        }

        public string Value
        {
            get => valueString;
            set
            {
                this.valueString = value;
                valueLabel.text = value;
            } 
        }
    }
}
