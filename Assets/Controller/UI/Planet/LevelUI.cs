using System.Collections.Generic;
using Bserg.Controller.Tools;
using Bserg.View.Custom.Level;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class LevelUI : UIClass
    {
        private VisualElement levelList;

        public LevelUI(VisualElement ui) : base(ui)
        {
            levelList = ui.Q<VisualElement>("level-list");
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
        }
    }
}