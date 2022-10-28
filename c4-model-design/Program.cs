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

            user.Uses(monitoringSystem, "Realiza consultas para mantenerse al tanto de la planificación de los vuelos hasta la llegada del lote de vacunas al Perú");
            admin.Uses(monitoringSystem, "Realiza consultas para mantenerse al tanto de la planificación de los vuelos hasta la llegada del lote de vacunas al Perú");

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
            Container webApplication = monitoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard con el resumen de toda la información del traslado de los lotes de vacunas.", "React");
            Container landingPage = monitoringSystem.AddContainer("Landing Page", "", "React");
            Container apiRest = monitoringSystem.AddContainer("API REST", "API Rest", "NodeJS (NestJS) port 8080");

            Container searchContext = monitoringSystem.AddContainer("Search Context", "Bounded Context de Busqueda de supermercados", "NodeJS (NestJS)");
            Container compareContext = monitoringSystem.AddContainer("Compare Context", "Bounded Context de Comparacion de precios", "NodeJS (NestJS)");
            Container mappingContext = monitoringSystem.AddContainer("Mapping Context", "Bounded Context de mapeo de los supermercados ", "NodeJS (NestJS)");
            Container securityContext = monitoringSystem.AddContainer("Security Context", "Bounded Context de Seguridad", "NodeJS (NestJS)");

            Container database = monitoringSystem.AddContainer("Database", "", "Oracle");

            user.Uses(webApplication, "Consulta");
            user.Uses(landingPage, "Consulta");

            admin.Uses(webApplication, "Consulta");
            admin.Uses(landingPage, "Consulta");

            webApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(searchContext, "", "");
            apiRest.Uses(compareContext, "", "");
            apiRest.Uses(mappingContext, "", "");
            apiRest.Uses(securityContext, "", "");

            searchContext.Uses(database, "", "");
            compareContext.Uses(database, "", "");
            mappingContext.Uses(database, "", "");
            securityContext.Uses(database, "", "");

            mappingContext.Uses(googleMaps, "API Request", "JSON/HTTPS");
            searchContext.Uses(supermercado, "API Request", "JSON/HTTPS");
            compareContext.Uses(supermercado, "API Request", "JSON/HTTPS");

            // Tags
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");

            string contextTag = "Context";

            searchContext.AddTags(contextTag);
            compareContext.AddTags(contextTag);
            mappingContext.AddTags(contextTag);
            securityContext.AddTags(contextTag);

            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIRest") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("Database") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle(contextTag) { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(monitoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            // 3. Diagrama de Componentes (mapping Context)
            Component domainLayer = mappingContext.AddComponent("Domain Layer", "", "NodeJS (NestJS)");
            Component mappingController = mappingContext.AddComponent("MappingController", "REST API endpoints de monitoreo.", "NodeJS (NestJS) REST Controller");
            Component monitoringApplicationService = mappingContext.AddComponent("MonitoringApplicationService", "Provee métodos para la localizacion, pertenece a la capa Application de DDD", "NestJS Component");
            Component locationRepository = mappingContext.AddComponent("LocationRepository", "Ubicación del supermercado", "NestJS Component");

            apiRest.Uses(mappingController, "", "JSON/HTTPS");
            mappingController.Uses(monitoringApplicationService, "Invoca métodos de monitoreo");

            monitoringApplicationService.Uses(domainLayer, "Usa", "");
            monitoringApplicationService.Uses(locationRepository, "", "");

            locationRepository.Uses(database, "", "");

            locationRepository.Uses(googleMaps, "", "JSON/HTTPS");

            // Tags
            domainLayer.AddTags("DomainLayer");
            mappingController.AddTags("MonitoringController");
            monitoringApplicationService.AddTags("MonitoringApplicationService");
            locationRepository.AddTags("LocationRepository");

            styles.Add(new ElementStyle("DomainLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightStatus") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("VaccineLoteRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("LocationRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AircraftSystemFacade") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView1 = viewSet.CreateComponentView(mappingContext, "Components1", "Component Diagram");
            componentView1.PaperSize = PaperSize.A4_Landscape;
            componentView1.Add(webApplication);
            componentView1.Add(apiRest);
            componentView1.Add(database);
            componentView1.Add(googleMaps);
            componentView1.AddAllComponents();

            // 3. Diagrama de Componentes (security Context)
            Component securityComponent = securityContext.AddComponent("Security Component", "", "NodeJS (NestJS)");
            Component verifyUser= securityContext.AddComponent("MonitoringApplicationService", "Provee métodos para el monitoreo, pertenece a la capa Application de DDD", "NestJS Component");
            Component verifyUserRepository = securityContext.AddComponent("verifyUserRepository", "Información del Usuario", "NestJS Component");

            apiRest.Uses(securityComponent, "", "JSON/HTTPS");
            securityComponent.Uses(monitoringApplicationService, "Invoca métodos de monitoreo");

            monitoringApplicationService.Uses(securityComponent, "Usa", "");
            monitoringApplicationService.Uses(verifyUser, "", "");
            monitoringApplicationService.Uses(verifyUserRepository, "", "");

            locationRepository.Uses(database, "", "");

            locationRepository.Uses(googleMaps, "", "JSON/HTTPS");

            // Tags
            securityComponent.AddTags("securityComponent");
            verifyUser.AddTags("verifyUser");
            verifyUserRepository.AddTags("verifyUserRepository");

            styles.Add(new ElementStyle("securityComponent") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("verifyUser") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("verifyUserRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView2 = viewSet.CreateComponentView(securityContext, "Components2", "Component Diagram");
            componentView2.PaperSize = PaperSize.A4_Landscape;
            componentView2.Add(webApplication);
            componentView2.Add(apiRest);
            componentView2.Add(database);
            componentView2.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}