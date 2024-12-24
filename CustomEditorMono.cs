using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace VulpesTool.Editor
{
    [CustomEditor(typeof(VulpesMonoBehaviour), true), CanEditMultipleObjects]
    public class CustomEditorMono : CustomGUIEditor
    {

    }
}
