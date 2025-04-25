using ES2Real.Models;

namespace ES2Real.Interfaces
{
    public interface IUtilizadorService
    {
        Task<UtilizadorAuth?> AuthenticateUserAsync(string email, string password);
        Task<UtilizadorAuth?> RegisterUserAsync(string username, string email, string password, string tipoUsuario);
        Task<UtilizadorAuth?> ObterPorEmail(string email);
        Task<List<Participante>?> ObterParticipantes();
        Task<List<Organizador>?> ObterOrganizadores();
        Task<bool> AtualizarParticipante(int idparticipante, string nome, string contacto, DateTime dataNascimento);
        Task<bool> AtualizarOrganizador(int idorganizador, string nome, string contacto, DateTime dataNascimento);
    }
}