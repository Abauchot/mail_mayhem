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
        
        // Initially: Square->0, Triangle->1, Circle->2, Diamond->3
        private int[] _symbolToSlot = { 0, 1, 2, 3 };

        public event Action OnMappingChanged;

        private void Awake()
        {
            InitializeMapping();
        }

        private void InitializeMapping()
        {
            // Set up initial mapping based on box configuration
            for (int slot = 0; slot < boxes.Length; slot++)
            {
                if (boxes[slot] == null) continue;
                int symbol = (int)boxes[slot].SymbolType;
                if (symbol >= 0 && symbol < _symbolToSlot.Length)
                    _symbolToSlot[symbol] = slot;
            }
        }

        public void ResetMapping()
        {
            // Reset to identity mapping and update boxes
            for (int i = 0; i < 4; i++)
                _symbolToSlot[i] = i;

            RefreshBoxSymbols();
            OnMappingChanged?.Invoke();
        }

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

            InitializeMapping();
        }

        public ServiceBox GetBox(SymbolType type)
        {
            int symbol = (int)type;
            if (symbol < 0 || symbol >= _symbolToSlot.Length) return null;

            int slot = _symbolToSlot[symbol];
            if (slot < 0 || slot >= boxes.Length) return null;

            return boxes[slot];
        }

        public void SwapBoxSymbols(SymbolType a, SymbolType b)
        {
            if (a == b) return;

            int idxA = (int)a;
            int idxB = (int)b;

            // Get current slots for each symbol
            int slotA = _symbolToSlot[idxA];
            int slotB = _symbolToSlot[idxB];

            // Swap the mapping
            _symbolToSlot[idxA] = slotB;
            _symbolToSlot[idxB] = slotA;

            // Update the boxes to accept their new symbols
            if (boxes[slotA] != null)
                boxes[slotA].SetAcceptedSymbol(b);

            if (boxes[slotB] != null)
                boxes[slotB].SetAcceptedSymbol(a);

            OnMappingChanged?.Invoke();
        }

        private void RefreshBoxSymbols()
        {
            // Update each box to accept the symbol that maps to its slot
            for (int symbol = 0; symbol < 4; symbol++)
            {
                int slot = _symbolToSlot[symbol];
                if (slot >= 0 && slot < boxes.Length && boxes[slot] != null)
                {
                    boxes[slot].SetAcceptedSymbol((SymbolType)symbol);
                }
            }
        }

        public SymbolType GetSymbolAtSlot(int slot)
        {
            for (int symbol = 0; symbol < _symbolToSlot.Length; symbol++)
            {
                if (_symbolToSlot[symbol] == slot)
                    return (SymbolType)symbol;
            }
            return (SymbolType)slot;
        }
    }
}
