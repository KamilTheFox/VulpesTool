using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VulpesTool.Editor
{
    public static class BoxDrawerState
    {
        public static Stack<(float y, Color color)> BoxStack = new Stack<(float, Color)>();
        public static List<(Rect rect, float startY, Color color)> PendingBoxes = new List<(Rect, float, Color)>();
        public static Color oldColor;
    }

    [CustomPropertyDrawer(typeof(BeginBoxAttribute))]
    public class BeginBoxDrawer : DecoratorDrawer
    {
        private const float SPACE_SIZE = 7f;

        public override void OnGUI(Rect position)
        {
            var attr = (BeginBoxAttribute)attribute;

            BoxDrawerState.oldColor = GUI.color;

            GUI.color = attr.ColorContent.GetColorFromFlags();

            BoxDrawerState.BoxStack.Push((position.y + SPACE_SIZE, attr.Color.GetColorFromFlags()));

            if (!string.IsNullOrEmpty(attr.Title))
            {
                EditorGUI.LabelField(
                    new Rect(position.x, position.y + SPACE_SIZE, position.width, 20),
                    attr.Title,
                    EditorStyles.boldLabel
                );
            }
        }

        public override float GetHeight()
        {
            var attr = (BeginBoxAttribute)attribute;
            return (!string.IsNullOrEmpty(attr.Title) ? 25 : SPACE_SIZE) + SPACE_SIZE;
        }
    }

    [CustomPropertyDrawer(typeof(EndBoxAttribute))]
    public class EndBoxDrawer : DecoratorDrawer
    {
        private const float SPACE_SIZE = 7f;

        public override void OnGUI(Rect position)
        {
            if (BoxDrawerState.BoxStack.Count > 0)
            {
                var (startY, boxColor) = BoxDrawerState.BoxStack.Pop();
                BoxDrawerState.PendingBoxes.Add((position, startY, boxColor));

                if (BoxDrawerState.BoxStack.Count == 0)
                {
                    for (int i = BoxDrawerState.PendingBoxes.Count - 1; i >= 0; i--)
                    {
                        var box = BoxDrawerState.PendingBoxes[i];
                        DrawBox(box.rect, box.startY, box.color);
                    }
                    BoxDrawerState.PendingBoxes.Clear();
                }
            }
            GUI.color = BoxDrawerState.oldColor;
        }

        public override float GetHeight() => SPACE_SIZE;

        private static void DrawBox(Rect position, float startY, Color boxColor)
        {
            var boxRect = new Rect(
                position.x - 5,
                startY - 5,
                position.width + 10,
                position.y - startY + 10
            );
            int oldDepth = GUI.depth;
            GUI.depth = oldDepth + 2;

            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = boxColor;
            GUI.Box(boxRect, "", EditorStyles.helpBox);
            GUI.backgroundColor = defaultColor;

            GUI.depth = oldDepth;
        }

        public static void CheckEndBox()
        {
            if (BoxDrawerState.BoxStack.Count == 0) return;

            var rect = EditorGUILayout.GetControlRect();
            var (startY, boxColor) = BoxDrawerState.BoxStack.Pop();
            DrawBox(rect, startY, boxColor);
        }
    }
}
