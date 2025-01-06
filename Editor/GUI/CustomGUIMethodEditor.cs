using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    public class CustomGUIMethodEditor : IDrawerGUI
    {
        private Dictionary<string, bool> groupFoldouts = new Dictionary<string, bool>();

        private UnityEngine.Object target;

        private List<(string, List<MethodInfo>)> methods;

        public void OnEnable(UnityEditor.Editor editor)
        {
            target = editor.target;

            methods = target.GetType()
                .GetAllMethods()
                .Where(m => m.GetCustomAttribute<CreateGUIAttribute>() != null)
                .OrderBy(m => m.GetCustomAttribute<CreateGUIAttribute>().Order)
                .GroupBy(m => m.GetCustomAttribute<CreateGUIAttribute>().Group)
                .Select(group => (group.Key, group.ToList())).ToList();
        }

        public void OnInspectorGUI()
        {
            if (VulpesUtils.IsVulpesObject() == false)
            {
                return;
            }
            methods.ForEach(MethodForeach);
        }

        private void MethodForeach((string, List<MethodInfo>) group)
        {
            string nameMethod = group.Item1;

            List<MethodInfo> methodsInfo = group.Item2;

            bool shouldDrawGroup = true;

            if (!string.IsNullOrEmpty(nameMethod))
            {
                if (!groupFoldouts.ContainsKey(nameMethod))
                    groupFoldouts[nameMethod] = true;

                groupFoldouts[nameMethod] = EditorGUILayout.Foldout(groupFoldouts[nameMethod], nameMethod);
                shouldDrawGroup = groupFoldouts[nameMethod];
                EditorGUI.indentLevel++;
            }

            if (shouldDrawGroup)
                methods.ForEach((tuple) => tuple.Item2.ForEach(DrawGUIMethod));

            if (!string.IsNullOrEmpty(nameMethod))
            {
                EditorGUI.indentLevel--;
            }
        }

        private void DrawGUIMethod(MethodInfo method)
        {
            var attr = method.GetCustomAttribute<CreateGUIAttribute>();
            string buttonName = attr.Title ?? method.Name;

            Color defaultColor = GUI.color;
            GUI.color = attr.Color.GetColorFromFlags();

            if (!attr.IsWindow)
            {
                EditorGUILayout.BeginVertical("box");

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
            }
            GUI.color = defaultColor;
        }
    }
}
