using System.Collections.Generic;
using UnityEngine;

namespace Bserg.Controller.Material
{
    public struct ElementStyle
    {
        public string Name;
        public string Symbol;
        public Color Color;

        private ElementStyle(string symbol, string name, Color color)
        {
            Name = name;
            Symbol = symbol;
            Color = color;

        }
        
        private static Dictionary<string, ElementStyle> styles = new();

        public static ElementStyle Get(string symbol) => styles[symbol];
        public static ElementStyle Get(ElementCount elementCount) => styles[elementCount.Symbol];

        private static void Add(ElementStyle style)
        {
            styles.Add(style.Symbol, style);
        }
        public static void Load()
        {
            if (styles.Count != 0)
                return;
            Add(new ElementStyle("Fe", "Iron", Color.HSVToRGB(0, 0, .6f)));
            Add(new ElementStyle("Si", "Silicon", Color.HSVToRGB(0, .6f , .6f)));
            Add(new ElementStyle("C", "Carbon", Color.HSVToRGB(1f/3f, .5f, .4f)));
            Add(new ElementStyle("MJ", "Power", Color.HSVToRGB(1f/6f, 1, .6f)));
        }
    }


    public struct ElementCount
    {
        public string Symbol;
        public int Count;

        public ElementCount(string symbol, int count)
        {
            Symbol = symbol;
            Count = count;
        }
    }
}