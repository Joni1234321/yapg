using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Custom.Transfer;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    /// <summary>
    /// Controls transfer view
    /// </summary>
    public class TransferUI : UIClass
    {
        private VisualElement transferList;
        public TransferUI (VisualElement root) : base(root)
        {
            transferList = root.Q<VisualElement>("transfer-list");
        }
        
        /// <summary>
        /// Updates teh visuals for transfers
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="planetNames"></param>
        /// <param name="transfers"></param>
        /// <param name="hohmannDeltaV"></param>
        public void UpdateTransfers(int planetID, string[] planetNames, HohmannTransfer[,] transfers, float[,] hohmannDeltaV)
        {
            transferList.Clear();
            
            string departureName = planetNames[planetID];
            
            // For each planet
            for (int destinationID = 0; destinationID < planetNames.Length; destinationID++)
            {
                if (destinationID == planetID) continue;

                HohmannTransfer transfer = transfers[planetID, destinationID];
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
    }
}