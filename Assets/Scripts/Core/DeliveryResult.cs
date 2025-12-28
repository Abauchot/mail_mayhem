using Letters;

namespace Core
{
    public readonly struct DeliveryResult
    {
        public readonly SymbolType expectedSymbolType;
        public readonly SymbolType deliveredSymbolType;
        public readonly bool isCorrect;
        
        public DeliveryResult(SymbolType expectedSymbolType, SymbolType deliveredSymbolType, bool isCorrect)
        {
            this.expectedSymbolType = expectedSymbolType;
            this.deliveredSymbolType = deliveredSymbolType;
            this.isCorrect = isCorrect;
        }
    }
}