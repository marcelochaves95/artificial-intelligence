using System;
using UnityEngine;

namespace NodeGraph
{
    [Serializable]
    public class Pin
    {
        [SerializeField]
        public string Name { get; set; }
        [SerializeField]
        public string Type { get; set; }
        [SerializeField]
        public Pin Next { get; set; }
    }
}