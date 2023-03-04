using System.Collections.Generic;
using Bserg.Controller.Material;
using Bserg.Controller.Tools;
using Bserg.Model.Space;
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

        public PlanetUI(VisualElement ui, BuildUI buildUI) : base(ui)
        {
            // Load materials
            elementRound = Resources.Load<VisualTreeAsset>("View/Material/element-round");
            elementSquareAsset = Resources.Load<VisualTreeAsset>("View/Material/element-square");

            // View
            nameLabel = ui.Q<Label>("name");
            
            // Population
            populationLabel = ui.Q<Label>("population");
                
            spacecraftPoolLabel = ui.Q<Label>("spacecraft-pool");

            groups = ui.Q<VisualElement>("groups");
            groups.Clear();
            
            planetPopulation = buildUI.CreateLevelGroup("Population", false, true, "Pop");
            planetFood = buildUI.CreateLevelGroup("Food");
            planetHousing = buildUI.CreateLevelGroup("Housing");
            planetLand = buildUI.CreateLevelGroup("Land");
            
            groups.Add(planetPopulation);
            groups.Add(planetHousing);
            groups.Add(planetFood);
            groups.Add(planetLand);
            
            elementList = ui.Q<VisualElement>("element-list");
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