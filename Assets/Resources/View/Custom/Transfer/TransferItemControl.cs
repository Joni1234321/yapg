using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.View.Custom.Transfer
{
    public class TransferItemControl : VisualElement
    {
        public TransferItemControl()
        {
            VisualElement body = Resources.Load<VisualTreeAsset>($"View/Custom/Transfer/transfer-item").CloneTree();
            Add(body);
            titleLabel = body.Q<Label>("transfer-info");
            windowLabel = body.Q<Label>("transfer-next");
            durationLabel = body.Q<Label>("transfer-duration");
        }
        
        public new class UxmlFactory : UxmlFactory<TransferItemControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription titleInput = new() { name = "title", defaultValue = "EARTH - MARS" };
            private readonly UxmlStringAttributeDescription windowInput = new() { name = "window", defaultValue = "28 Months" };
            private readonly UxmlStringAttributeDescription durationInput = new() { name = "duration", defaultValue = "12 Months" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as TransferItemControl;

                ate.Title = titleInput.GetValueFromBag(bag, cc);
                ate.Window = windowInput.GetValueFromBag(bag, cc);
                ate.Duration = durationInput.GetValueFromBag(bag, cc);
            }
        }

        private Label titleLabel, windowLabel, durationLabel;


        private string title, window, duration;

        public string Title
        {
            get => title;
            set
            {
                title = value;
                titleLabel.text = value;
            }
        }

        public string Window
        {
            get => window;
            set
            {
                window = value;
                windowLabel.text = value;
            }
        }

        public string Duration
        {
            get => duration;
            set
            {
                duration = value;
                durationLabel.text = value;
            } 
        }
    }


}
