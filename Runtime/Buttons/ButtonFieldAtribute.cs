using System;
using UnityEngine;

namespace VulpesTool
{
    public enum ButtonPosition : byte
    {
        Before,
        After,
        Left,
        Right
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class ButtonFieldAttribute : PropertyAttribute
    {
        public string MethodName { get; private set; }
        public string ButtonText { get; private set; }
        public ColorsGUI Color { get; private set; }
        public ButtonPosition Position { get; private set; }
        public float ButtonWidth { get; private set; }

        public bool IsChangeScene { get; private set; }

        public string IfEnable { get; private set; }

        public ButtonFieldAttribute(string methodName, string buttonText = null,
            ColorsGUI color = ColorsGUI.White, ButtonPosition position = ButtonPosition.Before,
            float buttonWidth = 60f, bool isChangeScene = false, string ifEnable = "")
        {
            MethodName = methodName;
            ButtonText = buttonText;
            Color = color;
            Position = position;
            ButtonWidth = buttonWidth;
            IsChangeScene = isChangeScene;
            IfEnable = ifEnable;
        }
    }
}
