using System;
using UnityEngine;

namespace VulpesTool
{
    [Serializable]
    public struct InterfaceReference<T> where T : class
    {
        [SerializeField] private UnityEngine.Object _reference;

        public T Interface
        {
            get
            {
                if (_reference == null) return null;

                if (_reference is T directInterface)
                    return directInterface;

                if (_reference is GameObject go)
                    return go.GetComponent<T>();

                if (_reference is Component comp)
                    return comp.GetComponent<T>();

                return null;
            }
        }

        public bool IsValid => Interface != null;
    }
}
