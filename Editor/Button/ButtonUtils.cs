using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace VulpesTool.Editor
{
    internal static class ButtonUtils
    {
        public static void ButtonEnable(Action button, string enableMethodName, UnityEngine.Object targetObject)
        {
            bool enableButton = true;
            if (!string.IsNullOrEmpty(enableMethodName))
            {
                var enabledMethod = targetObject.GetType()
                    .GetMethod(enableMethodName,
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

                if (enabledMethod != null && enabledMethod.ReturnType == typeof(bool))
                {
                    enableButton = (bool)enabledMethod.Invoke(targetObject, null);
                }
            }
            using (new EditorGUI.DisabledGroupScope(!enableButton))
            {
                button();
            }
        }
        public static void ClickButtonMethod(MethodInfo method, string buttonName, bool isSaveScene, params UnityEngine.Object[] targets)
        {
            foreach (var targetObject in targets)
            {
                if(targetObject is UnityEngine.Object target)
                {
                    Undo.RecordObject(target, $"Execute {buttonName}");
                    method.Invoke(target, null);
                    if (isSaveScene && targetObject is MonoBehaviour targetMono)
                        SaveScene(targetMono.gameObject);
                }
            }
        }

        public static void SaveScene(GameObject property)
        {
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(property.scene);
        }
    }
}
