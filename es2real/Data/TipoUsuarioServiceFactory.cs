using ES2Real.Interfaces;

namespace ES2Real.Data;

public class TipoUsuarioServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TipoUsuarioServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITipoUtilizadorService? GetService(string tipo)
    {
        return tipo switch
        {
            "Participante" => _serviceProvider.GetService<ParticipanteService>(),
            "Organizador" => _serviceProvider.GetService<OrganizadorService>(),
            _ => null
        };
    }
}
