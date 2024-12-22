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
        public static void ClickButtonMethod(MethodInfo method, string buttonName, bool isSaveScene, params UnityEngine.Object[] targets)
        {
            foreach (var targetObject in targets)
            {
                if(targetObject is MonoBehaviour targetMono)
                {
                    Undo.RecordObject(targetMono, $"Execute {buttonName}");
                    method.Invoke(targetMono, null);
                    if (isSaveScene)
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
