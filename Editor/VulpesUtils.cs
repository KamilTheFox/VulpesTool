using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace VulpesTool.Editor
{
    internal class VulpesUtils
    {
        public static bool IsVulpesObject(UnityEditor.SerializedProperty property)
        {
            var targetObject = property.serializedObject.targetObject;
            return targetObject is VulpesMonoBehaviour || targetObject is VulpesScriptableObject;
        }
        public static bool IsVulpesObject()
        {
            var editors = ActiveEditorTracker.sharedTracker.activeEditors;

            return editors.Any(editor =>
                editor.target is VulpesMonoBehaviour ||
                editor.target is VulpesScriptableObject);
        }
    }
}
