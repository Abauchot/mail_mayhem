using System.Collections.Generic;
using UnityEngine;
using Letters;

namespace Boxes
{
    public class BoxesRegistry : MonoBehaviour
    {
        [SerializeField] private List<ServiceBox> boxes = new();

        private readonly Dictionary<SymbolType, ServiceBox> _map = new();

        public IReadOnlyList<ServiceBox> Boxes => boxes;

        private void Awake()
        {
            Rebuild();
        }

        public void Rebuild()
        {
            _map.Clear();
            foreach (var b in boxes)
            {
                if (b == null) continue;
                _map[b.SymbolType] = b;
            }
        }

        public ServiceBox GetBox(SymbolType type)
        {
            return _map.GetValueOrDefault(type);
        }
    }
}