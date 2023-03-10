using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Bserg.View.Space;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Controller.Editor
{
    [CustomEditor(typeof(SystemGenerator))]
    public class SystemGeneratorEditor : UnityEditor.Editor
    {
        private string dataPath;
        public void OnEnable()
        {
            dataPath = Application.persistentDataPath + "/systems.json";
        }

        public override VisualElement CreateInspectorGUI()
        {
            
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            Button regenerateButton = new Button
            {
                text = "Regenerate"
            };
            regenerateButton.RegisterCallback<ClickEvent>(GenerateWorld);
            root.Add(regenerateButton);
            
            Button saveFileButton = new Button
            {
                text = "Save"
            };
            saveFileButton.RegisterCallback<ClickEvent>(_ => SaveFile());
            root.Add(saveFileButton);

            Button loadFileButton = new Button
            {
                text = "Load"
            };
            loadFileButton.RegisterCallback<ClickEvent>(_ => LoadFile());
            root.Add(loadFileButton);
            return root;
        }


        void SaveFile()
        {
            SystemGenerator systemGenerator = (SystemGenerator)target;
            string json = JsonUtility.ToJson(new SolarSystemData(systemGenerator.prefab.planetScriptables));
            File.WriteAllText(dataPath, json);
        }

        void LoadFile()
        {
            string jsonData = File.ReadAllText(dataPath);
            SolarSystemData color = JsonUtility.FromJson<SolarSystemData>(jsonData);
        }
        void GenerateWorld(ClickEvent e)
        {
            ((SystemGenerator)target).CreateSystem();
        }
    }
}
