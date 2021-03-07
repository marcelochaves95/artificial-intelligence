using System;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class EditorPinTypeInfo
    {
        [SerializeField]
        private string _type;

        public string Type => _type;

        public EditorPinTypeInfo(string type)
        {
            _type = type;
        }
    }
}