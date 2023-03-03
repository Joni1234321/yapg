using System.Collections.Generic;
using System.Linq;
using Bserg.Controller.Material;
using Bserg.Controller.Tools;
using Bserg.Model.Core;
using Bserg.Model.Core.Systems;
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
        private List<LevelGroupControl> inputs, outputs;
        private CounterControl buildLevel;
        
        private VisualElement elementList, levelList, transferList, migrationList, inputList, outputList;
        private readonly Label populationLabel, populationMigrationLabel, nameLabel, spacecraftPoolLabel;

        private readonly LevelControl planetLevel, planetAttraction;
        private readonly LevelGroupControl planetPopulation, planetHousing, planetFood;
        private VisualTreeAsset elementRound, elementSquareAsset;
        private FieldControl migrationTotalField;
        
        
        public UIPlanetController(Core.Controller controller, VisualElement planetUI, VisualElement buildUI, VisualElement levelUI, VisualElement transferUI, VisualElement migrationUI)
        {
            // Controller
            this.controller = controller;
            
            LevelStyle.Load();
            ElementStyle.Load();

            
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


            planetPopulation.BackgroundColor = LevelStyle.Get("Population").Color;
            planetFood.BackgroundColor = LevelStyle.Get("Food").Color;
            planetHousing.BackgroundColor = LevelStyle.Get("Housing").Color;
            
            inputList = buildUI.Q<VisualElement>("input-list");
            outputList = buildUI.Q<VisualElement>("output-list");
            buildLevel = buildUI.Q<CounterControl>();
            inputs = inputList.Query<LevelGroupControl>().ToList();
            outputs = outputList.Query<LevelGroupControl>().ToList();

            ChangeRecipe(housingRecipe, 0);
            
            planetPopulation.RegisterCallback<MouseUpEvent>(e => UpdateBuild(buildLevel.ValueInt));

            
            // Migration
            migrationList = migrationUI.Q<VisualElement>("migration-list");
            migrationTotalField = migrationUI.Q<FieldControl>("migration-total");

            // Load materials
            elementRound = Resources.Load<VisualTreeAsset>("View/Material/element-round");
            elementSquareAsset = Resources.Load<VisualTreeAsset>("View/Material/element-square");
            elementList = planetUI.Q<VisualElement>("element-list");
            
            // Load levels
            levelList = levelUI.Q<VisualElement>("level-list");
            
            // Load Transfers
            transferList = transferUI.Q<VisualElement>("transfer-list");
            
            // Trade menu
        }

        
        // 1 Land is approx 5 Acres (that is what one person needs)
        // Recipe is 1 pop + 4 land = 4 food
        // Meaning that 1 pop feeds 2^3 = 8 pops
        private Recipe foodRecipe =
            new(new[] { new RecipeItem("Population", 1), new RecipeItem("Land", 4) },
                new[] { new RecipeItem("Food", 4) });
        
        /* https://www.ti.org/vaupdate17.html
         *Perhaps in response to Cox's comments, on June 20 the Sierra Club modified the web page to compare four different densities:
* Dense urban, which is 400 households per acre or slightly less than the "efficient urban" of the day before;
* Efficient urban, which is "only" 100 households per acre;
* Efficient suburban, which is 10 households per acre; and
* Sprawl, which the Sierra Club defines as one household per acre.
         * 
         */
        // 1 housing per population
        // With low density
        // 5 homes per acre -> 25 homes per land -> 100 people per land
        // 1 person per 64 (2^6) people, for maintainance -> 2 people for 1 land
        private Recipe housingRecipe =
            new(new[] { new RecipeItem("Population", 1), new RecipeItem("Land", 0) },
                new[] { new RecipeItem("Housing", 7) });

        private Recipe currentRecipe;

        /// <summary>
        /// Changes the look of the buildmenu
        /// </summary>
        /// <param name="recipe"></param>
        void ChangeRecipe(Recipe recipe, int currentLevel)
        {
            currentRecipe = recipe;
            
            inputList.Clear();
            outputList.Clear();
            inputs.Clear();
            outputs.Clear();

            for (int i = 0; i < recipe.Input.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Input[i].Name);
                inputList.Add(group);
                inputs.Add(group);
            }
            
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Output[i].Name);
                outputList.Add(group);
                outputs.Add(group);
            }
            
            UpdateBuild(currentLevel);
        }

        /// <summary>
        /// Creates a group given the name, by looking up its levelstyle
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        LevelGroupControl CreateLevelGroup(string name)
        {
            LevelGroupControl group = new LevelGroupControl();

            LevelStyle style = LevelStyle.Get(name);
            group.ProgressEnabled = false;
            group.Text = style.Name;
            group.BackgroundColor = style.Color;
            return group;
        }
        
        /// <summary>
        /// Updates the UI's level count for items in recipe
        /// </summary>
        /// <param name="level"></param>
        void UpdateBuild(int level)
        {
            for (int i = 0; i < currentRecipe.Input.Length; i++)
                inputs[i].Level = (currentRecipe.Input[i].Level + level).ToString();
            
            for (int i = 0; i < currentRecipe.Output.Length; i++)
                outputs[i].Level = (currentRecipe.Output[i].Level + level).ToString();
        }
        
        public void SetPlanet(string name, float populationLevel, float populationMigration, float attractionLevel, int housingLevel, int foodLevel, long spacecraftPoolCount)
        {
            buildLevel.ValueInt = housingLevel;
            populationLabel.text = PrettyPrint.DecimalThousandsFormat(Util.LevelToLong(populationLevel));
            populationMigrationLabel.text = PrettyPrint.DecimalThousandsFormat(Util.LevelToLong(populationMigration));
            planetAttraction.Level = ((int)attractionLevel).ToString();
            planetPopulation.Level = ((int)populationLevel).ToString();
            
            // Food
            planetFood.Level = foodLevel.ToString();
            
            planetPopulation.Value = (populationLevel - (int)populationLevel) * 100f;
            planetHousing.Value = (housingLevel - (int)housingLevel) * 100f;
            planetHousing.Level = ((int)housingLevel).ToString();
            spacecraftPoolLabel.text = spacecraftPoolCount.ToString();
            nameLabel.text = name;
            UpdateBuild(buildLevel.ValueInt);
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
        public void UpdateTransfers(int planetID, string[] planetNames, HohmannTransfer<float>[,] transfers, float[,] hohmannDeltaV)
        {
            transferList.Clear();
            
            string departureName = planetNames[planetID];
            
            // For each planet
            for (int destinationID = 0; destinationID < planetNames.Length; destinationID++)
            {
                if (destinationID == planetID) continue;

                HohmannTransfer<float> transfer = transfers[planetID, destinationID];
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
