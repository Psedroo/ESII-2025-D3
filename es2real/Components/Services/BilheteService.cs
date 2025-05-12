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

        public Bilhete CriarBilhete(TipoBilhete tipo)
        {
            var factory = BilheteFactory.GetFactory(tipo); // usa factory method
            var bilhete = factory.CriarBilhete();

            _context.Bilhetes.Add(bilhete);
            _context.SaveChanges();

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