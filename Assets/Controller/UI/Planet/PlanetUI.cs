using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Controller.Sensors;
using Bserg.Controller.Tools;
using Bserg.Model.Space;
using Bserg.Model.Utilities;
using Bserg.View.Custom.Level;
using Model.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class PlanetUI : UIClass
    {
        private VisualElement elementList, groups;
        
        private readonly Label populationLabel, nameLabel, spacecraftPoolLabel;
        private readonly LevelGroupControl planetPopulation, planetHousing, planetFood, planetLand;

        private VisualTreeAsset elementRound, elementSquareAsset;

        public PlanetUI(VisualElement root, BuildUI buildUI, BuildSensor sensor) : base(root)
        {
            // Load materials
            elementRound = Resources.Load<VisualTreeAsset>("View/Material/element-round");
            elementSquareAsset = Resources.Load<VisualTreeAsset>("View/Material/element-square");

            // View
            nameLabel = root.Q<Label>("name");
            
            // Population
            populationLabel = root.Q<Label>("population");
                
            spacecraftPoolLabel = root.Q<Label>("spacecraft-pool");

            groups = root.Q<VisualElement>("groups");
            groups.Clear();
            
            planetPopulation = buildUI.CreateLevelGroup("Population", _ => sensor.ChangeRecipe(Recipe.Get("Population")),false, true, "Pop");
            planetFood = buildUI.CreateLevelGroup("Food", _ => sensor.ChangeRecipe(Recipe.Get("Food")));
            planetHousing = buildUI.CreateLevelGroup("Housing", _ => sensor.ChangeRecipe(Recipe.Get("Housing")));
            planetLand = buildUI.CreateLevelGroup("Land", _ => sensor.ChangeRecipe(Recipe.Get("Land")));
            
            groups.Add(planetPopulation);
            groups.Add(planetHousing);
            groups.Add(planetFood);
            groups.Add(planetLand);
            
            elementList = root.Q<VisualElement>("element-list");
        }


        public void Update(string name, long spacecraftPoolCount, PlanetLevels planetLevels, int planetID, float populationProgress)
        {
            int populationLevel = planetLevels.Get("Population")[planetID];
            int landLevel = planetLevels.Get("Land")[planetID];
            int housingLevel = planetLevels.Get("Housing")[planetID];
            int foodLevel = planetLevels.Get("Food")[planetID];
            
            // Population
            planetPopulation.Level = populationLevel.ToString();
            planetPopulation.Value = populationProgress * 100f;

            // BUILD
            planetFood.Level = foodLevel.ToString();
            planetHousing.Level = housingLevel.ToString();
            planetLand.Level = landLevel.ToString();

            
            // Labels
            populationLabel.text = PrettyPrint.DecimalThousandsFormat(Util.LevelToLong(populationLevel + populationProgress));
            
            spacecraftPoolLabel.text = spacecraftPoolCount.ToString();
            nameLabel.text = name;

        }
        
        
        
        // OLD
        
        public void UpdateMaterials(List<ElementCount> elementCounts)
        {
            elementList.Clear();
            elementCounts.ForEach(d =>
            {
                ElementStyle style = ElementStyle.Get(d);
                VisualElement ve = elementSquareAsset.CloneTree();
                Label l = ve.Q<Label>("label");
                l.text = style.Symbol;
                l.style.borderBottomColor = style.Color;
                l.style.borderLeftColor = style.Color;
                l.style.borderTopColor = style.Color;
                l.style.borderRightColor = style.Color;
                
                ve.Q<Label>("value").text = PrettyPrint.DecimalThousandsFormat(d.Count, "0");
                
                elementList.Add(ve);
            });    
        }
        
        
    }
}