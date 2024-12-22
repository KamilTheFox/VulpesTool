using UnityEngine;
using System;

namespace VulpesTool
{

    [AttributeUsage(AttributeTargets.Field)]
    public class BeginBoxAttribute : PropertyAttribute
    {
        public ColorsGUI Color { get; private set; }

        public ColorsGUI ColorContent { get; private set; }
        public string Title { get; private set; }

        public BeginBoxAttribute(string title = "", ColorsGUI color = ColorsGUI.White, ColorsGUI colorContent = ColorsGUI.White)
        {
            Title = title;
            Color = color;
            ColorContent = colorContent;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EndBoxAttribute : PropertyAttribute { }
}
