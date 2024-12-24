using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    public class EnumSearchWindow : EditorWindow
    {
        private string searchString = "";
        private Vector2 scrollPosition;
        private SerializedProperty targetProperty;
        private bool showAll = true;
        private Dictionary<string, bool> categoryFoldouts = new Dictionary<string, bool>();

        private Rect dragRect;
        private const float DragHeight = 20f; 

        void OnEnable()
        {
            dragRect = new Rect(0, 0, position.width, DragHeight);
        }

        public static void Show(Rect buttonRect, SerializedProperty property)
        {
            var window = GetWindow<EnumSearchWindow>();
            window.titleContent = new GUIContent("Select " + property.type);
            window.targetProperty = property;
            window.minSize = new Vector2(300, 400);
            window.ShowAsDropDown(buttonRect, new Vector2(300, 400));
        }

        private void OnGUI()
        {
            if (targetProperty == null)
            {
                this.Close();
                return;
            }

            dragRect.width = position.width;

            EditorGUI.LabelField(
                   dragRect,
                   "Select Value",
                   new GUIStyle(GUI.skin.label)
                   {
                       alignment = TextAnchor.MiddleCenter
                   });
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(DragHeight);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUI.SetNextControlName("SearchField");
            searchString = EditorGUILayout.TextField("Search", searchString);
            if (focusSearchField)
            {
                GUI.FocusControl("SearchField");
                focusSearchField = false;
            }

            bool newShowAll = GUILayout.Toggle(showAll, "All", EditorStyles.toolbarButton);
            if (newShowAll != showAll)
            {
                showAll = newShowAll;
                if (showAll)
                {
                    categoryFoldouts.Clear();
                }
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            var enumType = targetProperty.GetEnumType();
            var fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public)
                .OrderBy(f => f.MetadataToken); 

            if (showAll)
            {
                foreach (var field in fields)
                {
                    var value = field.GetValue(null);
                    if (string.IsNullOrEmpty(searchString) ||
                        field.Name.ToLower().Contains(searchString.ToLower()))
                    {
                        DrawEnumValue(value, fields.ToArray());
                    }
                }
            }
            else
            {
                var groupedValues = new Dictionary<string, List<(FieldInfo Field, object Value)>>();
                string currentCategory = "Other";

                foreach (var field in fields)
                {
                    var categoryAttr = field.GetCustomAttribute<EnumCategoryAttribute>();
                    if (categoryAttr != null)
                    {
                        currentCategory = categoryAttr.Category;
                    }

                    if (!groupedValues.ContainsKey(currentCategory))
                    {
                        groupedValues[currentCategory] = new List<(FieldInfo, object)>();
                    }

                    var value = field.GetValue(null);
                    groupedValues[currentCategory].Add((field, value));
                }

                foreach (var category in groupedValues.Keys)
                {
                    if (!categoryFoldouts.ContainsKey(category))
                    {
                        categoryFoldouts[category] = false;
                    }

                    categoryFoldouts[category] = EditorGUILayout.Foldout(
                        categoryFoldouts[category], category);

                    if (categoryFoldouts[category])
                    {
                        foreach (var (field, value) in groupedValues[category])
                        {
                            if (string.IsNullOrEmpty(searchString) ||
                                field.Name.ToLower().Contains(searchString.ToLower()))
                            {
                                DrawEnumValue(value, fields.ToArray());
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawEnumValue(object value, FieldInfo[] fields)
        {
            var field = fields.FirstOrDefault(f => f.Name == value.ToString());
            var attr = field?.GetCustomAttribute<EnumCategoryAttribute>();

            string label = value.ToString();
            string tooltip = attr?.Description ?? "";

            if (GUILayout.Button(new GUIContent(label, tooltip)))
            {
                Undo.RecordObject(targetProperty.serializedObject.targetObject, $"Execute {targetProperty.serializedObject.targetObject}");
                targetProperty.intValue = (int)value;
                targetProperty.serializedObject.ApplyModifiedProperties();
                Close();
            }
        }

        private bool focusSearchField = true;

        private void OnDestroy()
        {
            targetProperty = null;
        }
    }
    public static class SerializedPropertyExtensions
    {
        public static Type GetEnumType(this SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            var field = targetObject.GetType().GetField(property.propertyPath,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            if (field != null)
            {
                return field.FieldType;
            }

            var propertyPaths = property.propertyPath.Split('.');
            var type = targetObject.GetType();

            foreach (var path in propertyPaths)
            {
                if (path.Contains("["))
                {
                    var arrayName = path.Substring(0, path.IndexOf("["));
                    field = type.GetField(arrayName,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field != null)
                    {
                        type = field.FieldType.GetElementType();
                    }
                }
                else
                {
                    field = type.GetField(path,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field != null)
                    {
                        type = field.FieldType;
                    }
                }
            }

            return type;
        }
        public static string GetCategory(this Enum value)
        {
            var enumType = value.GetType();
            var fields = enumType.GetFields()
                .OrderBy(f => f.MetadataToken);

            string currentCategory = "None";

            foreach (var field in fields)
            {
                var categoryAttr = field.GetCustomAttribute<EnumCategoryAttribute>();
                if (categoryAttr != null)
                {
                    currentCategory = categoryAttr.Category;
                }

                if (field.Name == value.ToString())
                {
                    return currentCategory;
                }
            }

            return currentCategory;
        }
    }
}
