using System;
using System.Linq;
using UnityEngine;
using Letters;

namespace Boxes
{
    public sealed class BoxesRegistry : MonoBehaviour
    {
        [SerializeField] private ServiceBox[] boxes = new ServiceBox[4];
        public ServiceBox[] Boxes => boxes;

        public void RebuildFromRoot(Transform root)
        {
            if (root == null) return;

            var found = root.GetComponentsInChildren<ServiceBox>(true);
            Array.Clear(boxes, 0, boxes.Length);

            foreach (var b in found)
            {
                int idx = (int)b.SymbolType;
                if (idx < 0 || idx >= boxes.Length) continue;
                boxes[idx] = b;
            }
        }

        public ServiceBox GetBox(SymbolType type)
        {
            var idx = (int)type;
            if (idx < 0 || idx >= boxes.Length) return null;
            return boxes[idx];
        }
    }
}