using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CreateGUIAttribute : Attribute
    {
        public bool IsWindow { get; }
        public string Title { get; }
        public Vector2 MinSize { get; }
        public Vector2 MaxSize { get; }
        public ColorsGUI Color { get; }
        public bool IsDockable { get; }
        public bool IsUtility { get; }
        public bool IsModal { get; }
        public bool ShowInspector { get; }
        public int Order { get; } 
        public string Group { get; }

        public CreateGUIAttribute(
        bool isWindow = false,
        string title = null,
        ColorsGUI color = ColorsGUI.None,
        bool isDockable = true,
        bool isUtility = false,
        bool isModal = false,
        bool showInspector = true,
        int order = 0,
        string group = null,
        float minWidth = 250,
        float minHeight = 100,
        float maxWidth = 800,
        float maxHeight = 600)
        {
            IsWindow = isWindow;
            Title = title;
            Color = color;
            IsDockable = isDockable;
            IsUtility = isUtility;
            IsModal = isModal;
            ShowInspector = showInspector;
            Order = order;
            Group = group;
            MinSize = new Vector2(minWidth, minHeight);
            MaxSize = new Vector2(maxWidth, maxHeight);
        }

        // Конструктор с указанием размеров окна
        public CreateGUIAttribute(
            bool isWindow,
            float minWidth, float minHeight,
            float maxWidth = 800, float maxHeight = 600,
            string title = null,
            ColorsGUI color = ColorsGUI.None) : this(isWindow, title, color)
        {
            MinSize = new Vector2(minWidth, minHeight);
            MaxSize = new Vector2(maxWidth, maxHeight);
        }
    }
}
