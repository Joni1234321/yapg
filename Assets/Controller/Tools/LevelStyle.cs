using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using UnityEngine;

namespace Bserg.Controller.Tools
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

        public static List<string> GetAll() => styles.Keys.ToList();
        
        private static void Add(LevelStyle style)
        {
            styles.Add(style.Name, style);
        }
        public static void Load()
        {
            if (styles.Count != 0)
                return;
            
            Add(new LevelStyle("Economic", Color.HSVToRGB(0.14f, 1f, 0.6f)));
            Add(new LevelStyle("Population", Color.HSVToRGB(0, 0, .3f)));
            Add(new LevelStyle("Food", Color.HSVToRGB(80f/360f, 1f, .3f)));
            Add(new LevelStyle("Housing", Color.HSVToRGB(200f/360f, 1f, .4f)));
            Add(new LevelStyle("Land", Color.HSVToRGB(30f/360f, .4f, .6f)));
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