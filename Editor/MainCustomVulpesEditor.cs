using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace VulpesTool.Editor
{
    public class MainCustomVulpesEditor : UnityEditor.Editor
    {
        List<IDrawerGUI> drawerGUIs = new List<IDrawerGUI>()
        {
            new CustomGUIMethodEditor(),
            new ButtonDrawerGUI(),
        };

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            drawerGUIs.ForEach(x =>
            {
                x.OnEnable(this);
                x.OnInspectorGUI();
                x.OnDisable();
            });
        }

    }
}
