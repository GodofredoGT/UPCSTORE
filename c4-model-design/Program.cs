using Structurizr;
using Structurizr.Api;

namespace c4_model_design
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderModels();
        }

        static void RenderModels()
        {
            const long workspaceId = 77349;
            const string apiKey = "bbb30e1e-ab06-417e-a628-24d6cfdc4bd5";
            const string apiSecret = "4d7c482b-6ebf-4d04-92c4-93d0ba930708";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);

            Workspace workspace = new Workspace("Software Design & Patterns - C4 Model - Sistema de comparacion de precios", "UPC Store es un software que se encarga de comparar precios de diversos supermercados");

            ViewSet viewSet = workspace.Views;

            Model model = workspace.Model;

            // 1. Diagrama de Contexto
            SoftwareSystem monitoringSystem = model.AddSoftwareSystem("UPC Store System", "Permite al usuario comparar precios de productos alimenticios que se encuentran en diferentes supermercados");
            SoftwareSystem googleMaps = model.AddSoftwareSystem("Google Maps", "Plataforma que ofrece una REST API de información geo referencial.");
            SoftwareSystem supermercado = model.AddSoftwareSystem("SuperMercado", "Plataforma que ofrece productos alimentarios en ofertas");
           
            Person user = model.AddPerson("Usuario", "Usuario comun");
            Person admin = model.AddPerson("Admin", "User Admin.");

            user.Uses(monitoringSystem, "Realiza comparaciones sobre los precios de diversos productos de determinados supermercados del Perú");
            admin.Uses(monitoringSystem, "Administra los productos, los enlaces de los supermercados registrados");

            monitoringSystem.Uses(googleMaps, "Usa la API de google maps");
            monitoringSystem.Uses(supermercado, "Usa la API de Mercados y tiendas");

            // Tags
            user.AddTags("Usuario");
            admin.AddTags("Admin");
            monitoringSystem.AddTags("SistemaMonitoreo");
            googleMaps.AddTags("GoogleMaps");
            supermercado.AddTags("SuperMercado");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Usuario") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Admin") { Background = "#aa60af", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("GoogleMaps") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("AircraftSystem") { Background = "#2f95c7", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("SuperMercado") { Background="#2f95d7",Color="#ffffff",Shape=Shape.RoundedBox});

            SystemContextView contextView = viewSet.CreateSystemContextView(monitoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // 2. Diagrama de Contenedores
            Container webApplication = monitoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard en el que el cliente podrá comparar precios de productos.", "React");
            Container landingPage = monitoringSystem.AddContainer("Landing Page", "", "React");
            Container apiRest = monitoringSystem.AddContainer("API REST", "API Rest", "NodeJS (NestJS) port 8080");

            Container Cuenta = monitoringSystem.AddContainer("Cuenta", "Bounded Context de Cuenta", "NodeJS (NestJS)");
            Container Cliente = monitoringSystem.AddContainer("Cliente", "Bounded Context de Cliente", "NodeJS (NestJS)");
            Container Notificaciones = monitoringSystem.AddContainer("Notificaciones", "Bounded Context de Notificaciones", "NodeJS (NestJS)");
            Container Administracion = monitoringSystem.AddContainer("Administracion", "Bounded Context de Administracion", "NodeJS (NestJS)");
            Container Geolocalizacion = monitoringSystem.AddContainer("Geolocalizacion", "Bounded Context de mapeo de los supermercados ", "NodeJS (NestJS)");

            Container database = monitoringSystem.AddContainer("Database", "", "Oracle");

            user.Uses(webApplication, "Consulta");
            user.Uses(landingPage, "Consulta");

            admin.Uses(webApplication, "Consulta");
            admin.Uses(landingPage, "Consulta");

            webApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(Cuenta, "", "");
            apiRest.Uses(Cliente, "", "");
            apiRest.Uses(Geolocalizacion, "", "");
            apiRest.Uses(Notificaciones, "", "");
            apiRest.Uses(Administracion, "", "");

            Notificaciones.Uses(Cuenta, "", "");
            Notificaciones.Uses(Cliente, "", "");
            Notificaciones.Uses(Administracion, "", "");

            Geolocalizacion.Uses(Notificaciones, "", "");

            Cuenta.Uses(database, "", "");
            Cliente.Uses(database, "", "");
            Geolocalizacion.Uses(database, "", "");
            Administracion.Uses(database, "", "");

            Geolocalizacion.Uses(googleMaps, "API Request", "JSON/HTTPS");
            Cuenta.Uses(supermercado, "API Request", "JSON/HTTPS");
            Cliente.Uses(supermercado, "API Request", "JSON/HTTPS");

            // Tags
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");

            string contextTag = "Context";

            Cuenta.AddTags(contextTag);
            Cliente.AddTags(contextTag);
            Geolocalizacion.AddTags(contextTag);
            Administracion.AddTags(contextTag);
            Notificaciones.AddTags(contextTag);

            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIRest") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("Database") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle(contextTag) { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(monitoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            // 3. Diagrama de Componentes (Client Context)
            Component domainLayer = Cliente.AddComponent("Domain Layer", "", "NodeJS (NestJS)");
            Component clientController = Cliente.AddComponent("Client Controller", "REST API endpoints.", "NodeJS (NestJS) REST Controller");
            Component clientApplicationService = Cliente.AddComponent("Client Application Service", "Provee métodos que serán usadas por el cliente", "NestJS Component");
            Component compareRepository = Cliente.AddComponent("Compare Repository", "Comparación de precios de los poductos de super mercados", "NestJS Component");
            Component locationRepository = Cliente.AddComponent("Location Repository", "Localizar supermercados para luego redirigirlos", "NestJS Component");

            apiRest.Uses(clientController, "", "JSON/HTTPS");
            clientController.Uses(clientApplicationService, "");

            clientApplicationService.Uses(domainLayer, "Usa", "");
            clientApplicationService.Uses(compareRepository, "", "");
            clientApplicationService.Uses(locationRepository, "", "");

            compareRepository.Uses(supermercado, "Usa", "");
            compareRepository.Uses(database, "", "");

            locationRepository.Uses(googleMaps, "", "JSON/HTTPS");
            locationRepository.Uses(database, "", "");

            // Tags
            domainLayer.AddTags("DomainLayer");
            clientController.AddTags("ClientController");
            clientApplicationService.AddTags("ClientApplicationService");
            compareRepository.AddTags("CompareRepository");
            locationRepository.AddTags("LocationRepository");

            styles.Add(new ElementStyle("DomainLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ClientController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ClientApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ClientDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("CompareRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("LocationRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView = viewSet.CreateComponentView(Cliente, "Components", "Component Diagram");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(webApplication);
            componentView.Add(apiRest);
            componentView.Add(database);
            componentView.Add(googleMaps);
            componentView.Add(supermercado);
            componentView.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}