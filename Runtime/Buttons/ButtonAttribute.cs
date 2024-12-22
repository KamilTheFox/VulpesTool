using System;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public string ButtonName;

        public int Order { get; private set; }

        public ColorsGUI Color { get; private set; }

        public ButtonAttribute(string buttonName = "", int order = 0, ColorsGUI color = ColorsGUI.White)
        {
            ButtonName = buttonName;
            Order = order;
            Color = color;
        }
    }
    
}