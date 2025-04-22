public class BilheteService
{
    public Bilhete CriarBilhete(TipoBilhete tipo)
    {
        IBilheteFactory factory = tipo switch
        {
            TipoBilhete.Normal => new BilheteNormalFactory(),
            TipoBilhete.VIP => new BilheteVipFactory(),
            _ => throw new ArgumentException("Tipo de bilhete não suportado")
        };

        return factory.CriarBilhete();
    }
}