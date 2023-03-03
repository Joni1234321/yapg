using Bserg.View.Space;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Controller.Editor
{
    [CustomEditor(typeof(SystemGenerator))]
    public class SystemGeneratorEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            Button button = new Button
            {
                text = "Regenerate"
            };
            button.RegisterCallback<ClickEvent>(GenerateWorld);
            root.Add(button);

            return root;
        }


        void GenerateWorld(ClickEvent e)
        {
            ((SystemGenerator)target).CreateSystem();
        }
    }
}
