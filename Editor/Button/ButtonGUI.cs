using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VulpesTool.Editor
{
    public class ButtonDrawerGUI : IDrawerGUI
    {
        private static Type targetType;

        private IEnumerable<MethodInfo> methods;

        private UnityEditor.Editor editor;

        public void OnEnable(UnityEditor.Editor editor)
        {
            this.editor = editor;
            targetType = editor.target.GetType();
            methods = targetType.GetAllMethods()
                .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), false).Length > 0)
                .OrderBy(m => m.GetCustomAttribute<ButtonAttribute>().Order);
        }

        public Transform transform = null;
        public void OnInspectorGUI()
        {

            if (VulpesUtils.IsVulpesObject() == false)
                return;

            EndBoxDrawer.CheckEndBox();

            foreach (var method in methods)
            {
                var buttonAttr = method.GetCustomAttribute<ButtonAttribute>();
                string buttonName = string.IsNullOrEmpty(buttonAttr.ButtonName) ? method.Name : buttonAttr.ButtonName;
                Color defaultColor = GUI.color;

                GUI.color = buttonAttr.Color.GetColorFromFlags();


                ButtonUtils.ButtonEnable(() =>
                {
                    if (GUILayout.Button(buttonName))
                    {
                        ButtonUtils.ClickButtonMethod(method, buttonName, buttonAttr.IsChangeScene, editor.targets);
                    }
                },
                buttonAttr.IfEnable,
                editor.target);

                GUI.color = defaultColor;
            }
        }
    }
}
