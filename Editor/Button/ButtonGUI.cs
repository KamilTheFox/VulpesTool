using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VulpesTool.Editor
{
    [CustomEditor(typeof(UnityEngine.Object), true), CanEditMultipleObjects]
    public class ButtonAttributeEditor : UnityEditor.Editor
    {
        private static Type targetType;
        private IEnumerable<MethodInfo> methods;
        public void OnEnable()
        {
            targetType = target.GetType();
            methods = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), false).Length > 0)
                .OrderBy(m => m.GetCustomAttribute<ButtonAttribute>().Order);
        }

        public Transform transform = null;
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            EndBoxDrawer.CheckEndBox();

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                string buttonName = string.IsNullOrEmpty(buttonAttr.ButtonName) ? method.Name : buttonAttr.ButtonName;
                Color defaultColor = GUI.color;

                GUI.color = buttonAttr.Color.GetColorFromFlags();

                if (GUILayout.Button(buttonName))
                {
                    foreach (var targetObject in targets)
                    {
                        var targetMono = targetObject as MonoBehaviour;
                        if (targetMono != null)
                        {
                            Undo.RecordObject(targetMono, $"Execute {buttonName}");
                            method.Invoke(targetMono, null);
                        }
                    }
                }
                GUI.color = defaultColor;
            }
        }
    }
}