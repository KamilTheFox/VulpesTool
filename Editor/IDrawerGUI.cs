using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesTool.Editor
{
    internal interface IDrawerGUI
    {
        public void OnEnable(UnityEditor.Editor editor);

        public void OnInspectorGUI();

        public void OnDisable() { }

    }
}
