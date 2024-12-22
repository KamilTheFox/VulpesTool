using System;
using UnityEngine;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumSearchWindowAttribute : PropertyAttribute
    {
        public string IconName { get; private set; }

        public EnumSearchWindowAttribute(string iconName = "")
        {
            IconName = iconName;
        }
    }
}
