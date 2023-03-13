using Bserg.View.Custom.Field;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class MigrationUI : UIClass
    {
        private VisualElement migrationList;
        private FieldControl migrationTotalField;

        public MigrationUI (VisualElement root) : base(root) {
            migrationList = root.Q<VisualElement>("migration-list");
            migrationTotalField = root.Q<FieldControl>("migration-total");
        }
        
        /// <summary>
        /// Update migration UI
        /// </summary>
        /// <param name="departureID"></param>
        /// <param name="planetNames"></param>
        /// <param name="planetImmigration"></param>
        public void UpdateUI(int departureID, string[] planetNames, float[,] planetImmigration)
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