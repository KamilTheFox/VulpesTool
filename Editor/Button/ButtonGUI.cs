using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VulpesTool.Editor
{
    public class ButtonAttributeEditor : UnityEditor.Editor
    {
        private static Type targetType;
        private IEnumerable<MethodInfo> methods;
        public void OnEnable()
        {
            targetType = target.GetType();
            methods = targetType.GetAllMethods()
                .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), false).Length > 0)
                .OrderBy(m => m.GetCustomAttribute<ButtonAttribute>().Order);
        }

        public Transform transform = null;
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

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
                        ButtonUtils.ClickButtonMethod(method, buttonName, buttonAttr.IsChangeScene, targets);
                    }
                },
                buttonAttr.IfEnable,
                target);

                GUI.color = defaultColor;
            }
        }
    }
}
