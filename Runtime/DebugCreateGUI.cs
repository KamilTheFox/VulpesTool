using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesTool
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    [Obsolete("В разработке",true)]
    public class DebugOnlyAttribute : Attribute
    {
        public DebugOnlyAttribute()
        { }
    }

}
