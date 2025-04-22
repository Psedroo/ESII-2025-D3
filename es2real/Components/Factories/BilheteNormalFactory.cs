public class BilheteNormalFactory : IBilheteFactory
{
    public Bilhete CriarBilhete()
    {
        return new Bilhete
        {
            Tipo = TipoBilhete.Normal,
            Descricao = "Bilhete Normal",
            Quantidade = 100,
            Preco = 20.00m
        };
    }
}