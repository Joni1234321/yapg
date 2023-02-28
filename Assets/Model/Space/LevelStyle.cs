using System.Collections.Generic;
using UnityEngine;

namespace Bserg.Model.Space
{
    public struct LevelStyle
    {
        public string Name;
        public Color Color;
        
        private LevelStyle(string name, Color color)
        {
            Name = name;
            Color = color;
        }
        
        
        private static Dictionary<string, LevelStyle> styles = new();

        public static LevelStyle Get(string name) => styles[name];
        public static LevelStyle Get(LevelCount d) => styles[d.Name];

        private static void Add(LevelStyle style)
        {
            styles.Add(style.Name, style);
        }
        public static void Load()
        {
            if (styles.Count != 0)
                return;
            
            Add(new LevelStyle("Economic", Color.HSVToRGB(0.14f, 1f, 0.6f)));
            Add(new LevelStyle("Population", Color.HSVToRGB(1f/3f, .5f, .4f)));
            Add(new LevelStyle("Military", Color.HSVToRGB(0, .6f , .6f)));
            Add(new LevelStyle("Social", Color.HSVToRGB(.8f, .4f, .4f)));
            Add(new LevelStyle("Infrastructure", Color.HSVToRGB(0,0,.6f)));
        }
    }
    
    public struct LevelCount
    {
        public string Name;
        public int Count;

        public LevelCount(string name, int count)
        {
            Name = name;
            Count = count;
        }
    }
}