using ES2Real.Models;
using ES2Real.Data;
using ES2Real.Factories;
using System.Linq;

namespace ES2Real.Components.Services
{
    public class BilheteService
    {
        private readonly ApplicationDbContext _context;

        public BilheteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Bilhete CriarBilhete(TipoBilhete tipo, decimal preco, int quantidadeBilheteNormal)
        {
            var factory = BilheteFactory.GetFactory(tipo); // Usa factory method
            var bilhete = factory.CriarBilhete();
            bilhete.Preco = preco; // Set the ticket price
            bilhete.Quantidade = quantidadeBilheteNormal;

            // Do NOT add to context or save here; let the controller handle it
            return bilhete;
        }

        public bool CancelarBilhete(int id)
        {
            var bilhete = _context.Bilhetes.FirstOrDefault(b => b.Id == id);
            if (bilhete == null)
                return false;

            _context.Bilhetes.Remove(bilhete);
            _context.SaveChanges();
            return true;
        }
    }
}