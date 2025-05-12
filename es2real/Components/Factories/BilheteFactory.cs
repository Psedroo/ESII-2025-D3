using ES2Real.Models;

namespace ES2Real.Factories
{
    public static class BilheteFactory
    {
        public static IBilheteFactory GetFactory(TipoBilhete tipo)
        {
            return tipo switch
            {
                TipoBilhete.Normal => new BilheteNormalFactory(),
                TipoBilhete.VIP => new BilheteVipFactory(),
                _ => throw new ArgumentException("Tipo de bilhete inválido")
            };
        }
    }
}