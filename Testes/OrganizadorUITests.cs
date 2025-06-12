using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;

namespace Testes
{
    public class OrganizadorPageTests : TestContext
    {
        [Fact]
        public void DeveMostrarInformacoesDoOrganizadorAutenticado()
        {
            // Arrange
            var mockUsuario = new UtilizadorAuth
            {
                Username = "JoaoOrganizador",
                Email = "joao@email.com",
                TipoUsuario = "Organizador"
            };

            // Criar o mock do serviço de sessão
            var userSessionService = new UserSessionService();
            userSessionService.SetUsuario(mockUsuario);

            Services.AddSingleton<UserSessionService>(userSessionService);
            Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            
            var component = RenderComponent<ES2Real.Components.Pages.Organizador>();
            
            // Assert
            component.MarkupMatches(@$"
            <div class=""container mt-5"">
                <div class=""text-center mb-4"">
                    <h2 class=""fw-bold display-6"">
                        <i class=""bi bi-clipboard-data text-success me-2""></i>
                        Área do Organizador
                    </h2>
                    <p class=""text-muted mb-1"">
                        Bem-vindo, <strong>{mockUsuario.Username}</strong>
                    </p>
                    <p class=""text-muted"">Email: {mockUsuario.Email}</p>
                </div>
                <div class=""row row-cols-1 row-cols-md-3 g-4 justify-content text-center"">
                    <div class=""col-md-4"">
                        <a href=""/Eventos"" class=""btn btn-primary btn-lg w-100"">Gerir Eventos</a>
                    </div>
                    <div class=""col-md-4"">
                        <a href=""/Participantes"" class=""btn btn-secondary btn-lg w-100"">Gerir Participantes</a>
                    </div>
                    <div class=""col-md-4"">
                        <a href=""/Relatorios"" class=""btn btn-success btn-lg w-100"">Relatórios</a>
                    </div>
                    <div class=""col-md-4"">
                        <a href=""/Minhas-informacoes"" class=""btn btn-secondary btn-lg w-100"">Minhas Informações</a>
                    </div>
                    <div class=""col-md-4"">
                        <a href=""/Mensagens"" class=""btn btn-secondary btn-lg w-100"">Mensagens</a>
                    </div>
                </div>
            </div>
        ");

            component.Find("a[href='/Eventos']");
            component.Find("a[href='/Participantes']");
            component.Find("a[href='/Relatorios']");
            component.Find("a[href='/Minhas-informacoes']");
            component.Find("a[href='/Mensagens']");
        }
    }

    // Mock para NavigationManager, necessário para o teste
    public class MockNavigationManager : NavigationManager
    {
        public MockNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            //Navegação simulada
        }
    }
}
