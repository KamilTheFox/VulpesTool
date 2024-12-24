using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    
    public class CustomGUIEditor : ButtonAttributeEditor
    {
        private Dictionary<string, bool> groupFoldouts = new Dictionary<string, bool>();

        private void DrawGUIMethods()
        {
            var methods = target.GetType()
                .GetAllMethods()
                .Where(m => m.GetCustomAttribute<CreateGUIAttribute>() != null)
                .OrderBy(m => m.GetCustomAttribute<CreateGUIAttribute>().Order)
                .GroupBy(m => m.GetCustomAttribute<CreateGUIAttribute>().Group);

            foreach (var group in methods)
            {
                bool shouldDrawGroup = true;

                // Если есть группа, создаем foldout
                if (!string.IsNullOrEmpty(group.Key))
                {
                    if (!groupFoldouts.ContainsKey(group.Key))
                        groupFoldouts[group.Key] = true;

                    groupFoldouts[group.Key] = EditorGUILayout.Foldout(groupFoldouts[group.Key], group.Key);
                    shouldDrawGroup = groupFoldouts[group.Key];
                    EditorGUI.indentLevel++;
                }

                if (shouldDrawGroup)
                {
                    foreach (var method in group)
                    {
                        var attr = method.GetCustomAttribute<CreateGUIAttribute>();
                        string buttonName = attr.Title ?? method.Name;

                        if (!attr.IsWindow)
                        {
                            EditorGUILayout.BeginVertical("box");
                            Color defaultColor = GUI.color;
                            GUI.color = attr.Color.GetColorFromFlags();

                            var centeredStyle = new GUIStyle(GUI.skin.label)
                            {
                                alignment = TextAnchor.MiddleCenter
                            };

                            EditorGUILayout.LabelField(buttonName, centeredStyle);

                            GUITracker.StartTracking(target);
                            method.Invoke(target, null);
                            GUITracker.EndTracking();

                            GUI.color = defaultColor;
                            EditorGUILayout.EndVertical();
                        }
                        else
                        {
                            Color defaultColor = GUI.color;
                            GUI.color = attr.Color.GetColorFromFlags();

                            if (GUILayout.Button($"Open {buttonName}"))
                            {
                                CustomGUIWindow window;
                                if (attr.IsModal)
                                {
                                    window = ScriptableObject.CreateInstance<CustomGUIWindow>();
                                    window.ShowModalUtility();
                                }
                                else
                                {
                                    window = attr.IsUtility
                                        ? EditorWindow.GetWindow<CustomGUIWindow>(true, buttonName, true)
                                        : EditorWindow.GetWindow<CustomGUIWindow>(attr.IsDockable);
                                }

                                window.titleContent = new GUIContent(buttonName);
                                window.minSize = attr.MinSize;
                                window.maxSize = attr.MaxSize;
                                window.Init(target, method, attr);
                            }

                            GUI.color = defaultColor;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(group.Key))
                {
                    EditorGUI.indentLevel--;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (VulpesUtils.IsVulpesObject() == false)
            {
                return;
            }
            DrawGUIMethods();
        }
    }
}
