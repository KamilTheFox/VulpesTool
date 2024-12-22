using System;
using UnityEngine;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumCategoryAttribute : PropertyAttribute
    {
        public string Category { get; private set; }
        public string Description { get; private set; }

        public EnumCategoryAttribute(string category, string description = "")
        {
            Category = category;
            Description = description;
        }
    }
}
