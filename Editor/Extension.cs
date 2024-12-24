using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VulpesTool.Editor
{
    internal static partial class Extension
    {
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type,
      BindingFlags? bindingFlags = null,
      bool includeBaseTypes = true,
      bool excludeObjectMethods = true)
        {
            if (type == null) return Enumerable.Empty<MethodInfo>();

            var flags = bindingFlags ?? (BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Static | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly);

            var methods = new HashSet<MethodInfo>(new MethodInfoComparer());
            var currentType = type;

            while (currentType != null &&
                  (!excludeObjectMethods || currentType != typeof(object)))
            {
                var currentMethods = currentType.GetMethods(flags);

                foreach (var method in currentMethods)
                {
                    methods.Add(method);
                }

                if (!includeBaseTypes) break;
                currentType = currentType.BaseType;
            }

            return methods;
        }

        // Компаратор для предотвращения дубликатов методов
        private class MethodInfoComparer : IEqualityComparer<MethodInfo>
        {
            public bool Equals(MethodInfo x, MethodInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;

                return x.DeclaringType == y.DeclaringType &&
                       x.Name == y.Name &&
                       ParametersMatch(x, y);
            }

            private bool ParametersMatch(MethodInfo x, MethodInfo y)
            {
                var xParams = x.GetParameters();
                var yParams = y.GetParameters();

                if (xParams.Length != yParams.Length) return false;

                for (int i = 0; i < xParams.Length; i++)
                {
                    if (xParams[i].ParameterType != yParams[i].ParameterType)
                        return false;
                }

                return true;
            }

            public int GetHashCode(MethodInfo obj)
            {
                if (obj == null) return 0;

                var hash = new HashCode();
                hash.Add(obj.DeclaringType);
                hash.Add(obj.Name);

                foreach (var param in obj.GetParameters())
                {
                    hash.Add(param.ParameterType);
                }

                return hash.ToHashCode();
            }
        }
    }
}
