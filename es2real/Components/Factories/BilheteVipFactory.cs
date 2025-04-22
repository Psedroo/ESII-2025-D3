public class BilheteVipFactory : IBilheteFactory
{
    public Bilhete CriarBilhete()
    {
        return new Bilhete
        {
            Tipo = TipoBilhete.VIP,
            Descricao = "Bilhete VIP com acesso exclusivo",
            Quantidade = 50,
            Preco = 50.00m
        };
    }
}