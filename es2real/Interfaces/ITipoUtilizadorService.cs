namespace ES2Real.Interfaces;

public interface ITipoUtilizadorService
{
    Task CriarRegistoEspecificoAsync(UtilizadorAuth utilizador);
}