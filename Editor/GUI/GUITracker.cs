using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VulpesTool.Editor
{
    internal class GUITracker
    {
        private static bool isTracking;
        private static UnityEngine.Object currentTarget;
        private static int originalHash;

        public static void StartTracking(UnityEngine.Object target)
        {
            isTracking = true;
            currentTarget = target;
            originalHash = GetObjectHash(target);
        }

        public static void EndTracking()
        {
            if (isTracking && GUI.changed)
            {
                int newHash = GetObjectHash(currentTarget);

                if (newHash != originalHash)
                {
                    Undo.RecordObject(currentTarget, "GUI Change");

                    if (currentTarget is Component comp)
                        EditorSceneManager.MarkSceneDirty(comp.gameObject.scene);

                    EditorUtility.SetDirty(currentTarget);
                }
            }

            isTracking = false;
            currentTarget = null;
        }

        private static int GetObjectHash(UnityEngine.Object obj)
        {
            var hash = new HashCode();

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var value = field.GetValue(obj);
                if (value != null)
                {
                    hash.Add(value.GetHashCode());
                }
            }

            return hash.ToHashCode();
        }
    }
}
