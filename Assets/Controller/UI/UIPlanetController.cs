using System.Collections.Generic;
using System.Linq;
using Bserg.Controller.Material;
using Bserg.Controller.Tools;
using Bserg.Model.Core;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Custom.Counter;
using Bserg.View.Custom.Field;
using Bserg.View.Custom.Level;
using Bserg.View.Custom.Progress;
using Bserg.View.Custom.Transfer;
using Model.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using Time = Bserg.Model.Units.Time;

namespace Bserg.Controller.UI
{
    public class UIPlanetController
    {
        // Controller
        private Core.Controller controller;
        
        // View
        private List<VisualElement> buildList;
        private VisualElement elementList, levelList, transferList, migrationList;
        private readonly Label populationLabel, populationMigrationLabel, nameLabel, spacecraftPoolLabel;

        private readonly LevelControl planetLevel, planetAttraction;
        private readonly LevelGroupControl planetPopulation, planetHousing, planetFood;
        private VisualTreeAsset elementRound, elementSquareAsset;
        private FieldControl migrationTotalField;
        
        
        public UIPlanetController(Core.Controller controller, VisualElement planetUI, VisualElement buildUI, VisualElement levelUI, VisualElement transferUI, VisualElement migrationUI)
        {
            // Controller
            this.controller = controller;
            
            // View
            nameLabel = planetUI.Q<Label>("name");
            planetLevel = planetUI.Q<LevelControl>("planet-level");
            
            // Population
            populationLabel = planetUI.Q<Label>("population");
            populationMigrationLabel = planetUI.Q<Label>("population-migration");
            planetAttraction = planetUI.Q<LevelControl>("attraction-level");
            planetPopulation = planetUI.Q<LevelGroupControl>("population-group");
            planetHousing = planetUI.Q<LevelGroupControl>("housing-group");
            planetFood = planetUI.Q<LevelGroupControl>("food-group");
            
            planetAttraction = planetUI.Q<LevelControl>("attraction-level");
                
            spacecraftPoolLabel = planetUI.Q<Label>("spacecraft-pool");

            VisualElement artisanalMineButton = planetUI.Q<VisualElement>("artisanal-mine");
            //artisanalMineButton.RegisterCallback<MouseUpEvent>(Build);

            buildList = buildUI.Query(className: "build-item").ToList();
            
            // Migration
            migrationList = migrationUI.Q<VisualElement>("migration-list");
            migrationTotalField = migrationUI.Q<FieldControl>("migration-total");

            // Load materials
            elementRound = Resources.Load<VisualTreeAsset>("View/Material/element-round");
            elementSquareAsset = Resources.Load<VisualTreeAsset>("View/Material/element-square");
            ElementStyle.Load();
            elementList = planetUI.Q<VisualElement>("element-list");
            
            // Load levels
            LevelStyle.Load();
            levelList = levelUI.Q<VisualElement>("level-list");
            
            // Load Transfers
            transferList = transferUI.Q<VisualElement>("transfer-list");
            
            // Trade menu
        }

        public void SetPlanet(string name, float populationLevel, float populationMigration, float attractionLevel, float housingLevel, float foodLevel, long spacecraftPoolCount)
        {
            populationLabel.text = PrettyPrint.DecimalThousandsFormat(Util.LevelToLong(populationLevel));
            //populationDiffLabel.text = PrettyPrint.DecimalThousandsFormat(populationDiff);
            //populationGrowthLabel.text = PrettyPrint.DecimalThousandsFormat(populationGrowth);
            //populationDeclineLabel.text = PrettyPrint.DecimalThousandsFormat(populationDecline);
            populationMigrationLabel.text = PrettyPrint.DecimalThousandsFormat(Util.LevelToLong(populationMigration));
            planetAttraction.Level = ((int)attractionLevel).ToString();
            planetPopulation.Level = ((int)populationLevel).ToString();
            planetFood.Level = ((int)foodLevel).ToString();
            planetPopulation.Value = (populationLevel - (int)populationLevel) * 100f;
            planetHousing.Value = (housingLevel - (int)housingLevel) * 100f;
            planetFood.Value = (foodLevel - (int)foodLevel) * 100f;
            planetHousing.Level = ((int)housingLevel).ToString();
            spacecraftPoolLabel.text = spacecraftPoolCount.ToString();
            nameLabel.text = name;
        }

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

        public void UpdateLevels(List<LevelCount> levelCounts)
        {
            int total = 0;
            // Clear
            levelList.Clear();
            
            // Add each element
            levelCounts.ForEach(d =>
            {
                total += d.Count;
                
                LevelStyle style = LevelStyle.Get(d);
                
                LevelControl levelControl = new LevelControl
                {
                    LevelSize = LevelControl.LevelSizeEnum.Medium,
                    Level = d.Count.ToString(),
                    BackgroundColor = style.Color
                };
                levelList.Add(levelControl);
            });
            
            // Planet total level
            planetLevel.Level = total.ToString();
        }

        /// <summary>
        /// Updates teh visuals for transfers
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="planetNames"></param>
        /// <param name="transfers"></param>
        /// <param name="hohmannDeltaV"></param>
        public void UpdateTransfers(int planetID, string[] planetNames, Game.HohmannTransfer<float>[,] transfers, float[,] hohmannDeltaV)
        {
            transferList.Clear();
            
            string departureName = planetNames[planetID];
            
            // For each planet
            for (int destinationID = 0; destinationID < planetNames.Length; destinationID++)
            {
                if (destinationID == planetID) continue;

                Game.HohmannTransfer<float> transfer = transfers[planetID, destinationID];
                TransferItemControl transferItem = new TransferItemControl
                {
                    Title = departureName + " - " + planetNames[destinationID], 
                    Duration = GameTick.ToTime(transfer.Duration).To(Time.UnitType.Weeks).ToString("0") + " W "+ (hohmannDeltaV[planetID, destinationID]/1000).ToString("0.0") + "km/s",
                    Window = GameTick.ToTime(transfer.Window).To(Time.UnitType.Weeks).ToString("0") + " W ",
                    //(" + controller.Game.TicksUntilNextEventF(transfer.Window, transfer.Offset).ToString("0.0") + ")"
                    //  (" + transfer.Duration.ToString("0") + ") " 
                };
                transferList.Add(transferItem); 
            }   
        }

        /// <summary>
        /// Update migration UI
        /// </summary>
        /// <param name="departureID"></param>
        /// <param name="planetNames"></param>
        /// <param name="planetImmigration"></param>
        public void UpdateMigration(int departureID, string[] planetNames, float[,] planetImmigration)
        {
            float total = 0;
            migrationList.Clear();
            for (int destinationID = 0; destinationID < planetNames.Length; destinationID++)
            {
                //if (i == planetID) continue;
                FieldControl field = new FieldControl
                {
                    Title = planetNames[destinationID],
                    Value = planetImmigration[departureID, destinationID].ToString(),
                };
                migrationList.Add(field);

                total += planetImmigration[departureID, destinationID];
            }

            migrationTotalField.Value = total.ToString();
        }
    }
}
