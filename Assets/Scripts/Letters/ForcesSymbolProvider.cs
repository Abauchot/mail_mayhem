namespace Letters
{
    public sealed class ForcedSymbolProvider : ISymbolProvider
    {
        private readonly SymbolType _forced;
        public ForcedSymbolProvider(SymbolType forced) => _forced = forced;
        public SymbolType Next() => _forced;
    }
}