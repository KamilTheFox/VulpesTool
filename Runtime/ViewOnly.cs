using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ViewOnlyAttribute : PropertyAttribute
    {
        public bool OnlyInPlayMode { get; private set; }

        public ViewOnlyAttribute(bool onlyInPlayMode = false)
        {
            OnlyInPlayMode = onlyInPlayMode;
        }
    }
}
