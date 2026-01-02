using UnityEngine;

namespace Letters
{
    public sealed class RandomSymbolProvider : ISymbolProvider
    {
        private System.Random _rng;
        
        public RandomSymbolProvider(int? seed = null)
        {
            _rng = seed.HasValue ? new System.Random(seed.Value) : new System.Random();
        }

        public SymbolType Next()
        {
            int v = _rng.Next(0, 4);
            return (SymbolType)v;
        }
    }
}