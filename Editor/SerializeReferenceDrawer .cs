using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{

    [CustomPropertyDrawer(typeof(SelectImplementationAttribute))]
    internal class SerializeReferenceDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!property.propertyType.Equals(SerializedPropertyType.ManagedReference))
            {
                EditorGUI.LabelField(position, label.text, "Use SelectImplementation with SerializeReference only!");
                return;
            }

            var propertyRect = new Rect(position.x, position.y, position.width - 65, position.height);
            var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, EditorGUIUtility.singleLineHeight);

            bool isArrayElement = property.propertyPath.Contains("Array.data");

            string currentType = property.managedReferenceValue?.GetType().Name ?? "None";
            string interfaceTypeName = property.managedReferenceFieldTypename.Split(' ').LastOrDefault()?.Split('.').LastOrDefault() ?? "Unknown";

            string displayName;
            if (isArrayElement)
            {
                // Для элементов массива/списка показываем только типы
                displayName = $"{interfaceTypeName}: {currentType}";
            }
            else
            {
                // Для одиночных полей показываем полную информацию
                displayName = $"{label.text} - {interfaceTypeName}: {currentType}";
            }

            var newLabel = new GUIContent(displayName, label.tooltip);

            EditorGUI.PropertyField(propertyRect, property, newLabel, true);

            if (GUI.Button(buttonRect, "Select"))
            {
                string baseTypeName = property.managedReferenceFieldTypename;
                Type baseType = GetTypeFromSerializedPropertyTypename(baseTypeName);

                if (baseType != null)
                {
                    SelectImplementationWindow.Show(property, baseType);
                }
            }
        }

        private Type GetTypeFromSerializedPropertyTypename(string typeString)
        {
            string[] splits = typeString.Split(new char[] { ' ' }, 2);
            if (splits.Length != 2) return null;

            string assemblyName = splits[0];
            string typeName = splits[1];

            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName)?
                .GetType(typeName);
        }
    }
    public class SelectImplementationWindow : EditorWindow
    {
        private SerializedProperty targetProperty;
        private Type baseType;
        private Vector2 scrollPosition;
        private List<Type> implementations;
        private string searchString = "";

        public static void Show(SerializedProperty property, Type baseType)
        {
            var window = GetWindow<SelectImplementationWindow>("Select Type");
            window.targetProperty = property;
            window.baseType = baseType;
            window.LoadImplementations();
            window.minSize = new Vector2(200, 100);
        }

        private void LoadImplementations()
        {
            implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type)
                    && !type.IsInterface
                    && !type.IsAbstract
                    && type != baseType)
                .ToList();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Select {baseType.Name} Implementation", EditorStyles.boldLabel);

            // Добавляем поле поиска
            searchString = EditorGUILayout.TextField("Search:", searchString);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (GUILayout.Button("Null"))
            {
                targetProperty.managedReferenceValue = null;
                targetProperty.serializedObject.ApplyModifiedProperties();
                Close();
            }

            foreach (var impl in implementations)
            {
                // Фильтруем по поиску
                if (!string.IsNullOrEmpty(searchString)
                    && !impl.Name.ToLower().Contains(searchString.ToLower()))
                    continue;

                if (GUILayout.Button(impl.Name))
                {
                    var instance = Activator.CreateInstance(impl);
                    targetProperty.managedReferenceValue = instance;
                    targetProperty.serializedObject.ApplyModifiedProperties();
                    Close();
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
