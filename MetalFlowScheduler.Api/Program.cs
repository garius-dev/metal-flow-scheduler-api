using Asp.Versioning;
using MetalFlowScheduler.Api.Application.Services;
using MetalFlowScheduler.Api.Interfaces.Repositories;
using Microsoft.OpenApi.Models;
using Serilog.Events;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

// --- Configuração do Serilog ---
// Configura o logger estático global Log.Logger
// Isso é útil para logging durante a inicialização, antes que a injeção de dependência esteja pronta.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // Define o nível mínimo de log (ex: Debug, Information, Warning, Error)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information) // Reduz logs do próprio ASP.NET Core
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) // Reduz mais ainda logs do ASP.NET Core
    .Enrich.FromLogContext() // Adiciona informações de contexto aos logs
    .WriteTo.Console() // Escreve logs no console (bom para desenvolvimento)
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.File( // Escreve logs em um arquivo
        "MetalFlowScheduler-.log", // Nome do arquivo. O traço fará ele adicionar a data.
        rollingInterval: RollingInterval.Day, // Cria um novo arquivo a cada dia
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}", // Formato da saída
        restrictedToMinimumLevel: LogEventLevel.Information // Nível mínimo para ESTE sink (arquivo)
     )
    .CreateLogger();

Log.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

string devMode = "DevMock";

if (devMode == "DevMock")
{
    Console.WriteLine("INFO: Using Mock Repositories");
    // Initialize mock data once
    // Inicializar dados mock uma vez
    MetalFlowScheduler.Api.Infrastructure.Mocks.MockDataFactory.Initialize();

    // Register Mock Implementations
    // Registrar Implementações Mock
    builder.Services.AddScoped<ILineRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockLineRepository>();
    builder.Services.AddScoped<IWorkCenterRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockWorkCenterRepository>();
    builder.Services.AddScoped<IOperationRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockOperationRepository>();
    builder.Services.AddScoped<IProductRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockProductRepository>();
    builder.Services.AddScoped<IProductionOrderRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockProductionOrderRepository>();
    builder.Services.AddScoped<IProductionOrderItemRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockProductionOrderItemRepository>();
    builder.Services.AddScoped<ISurplusPerProductAndWorkCenterRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockSurplusPerProductAndWorkCenterRepository>();
    builder.Services.AddScoped<IOperationTypeRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockOperationTypeRepository>();
    builder.Services.AddScoped<IWorkCenterOperationRouteRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockWorkCenterOperationRouteRepository>();
    builder.Services.AddScoped<ILineWorkCenterRouteRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockLineWorkCenterRouteRepository>();
    builder.Services.AddScoped<IProductAvailablePerLineRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockProductAvailablePerLineRepository>();
    builder.Services.AddScoped<IProductOperationRouteRepository, MetalFlowScheduler.Api.Infrastructure.Mocks.MockProductOperationRouteRepository>();

}
else
{
    builder.Services.AddScoped<ILineRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.LineRepository>();
    builder.Services.AddScoped<IWorkCenterRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.WorkCenterRepository>();
    builder.Services.AddScoped<IOperationRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.OperationRepository>();
    builder.Services.AddScoped<IProductRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.ProductRepository>();
    builder.Services.AddScoped<IProductionOrderRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.ProductionOrderRepository>();
    builder.Services.AddScoped<IProductionOrderItemRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.ProductionOrderItemRepository>();
    builder.Services.AddScoped<ISurplusPerProductAndWorkCenterRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.SurplusPerProductAndWorkCenterRepository>();
    builder.Services.AddScoped<IOperationTypeRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.OperationTypeRepository>();
    builder.Services.AddScoped<IWorkCenterOperationRouteRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.WorkCenterOperationRouteRepository>();
    builder.Services.AddScoped<ILineWorkCenterRouteRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.LineWorkCenterRouteRepository>();
    builder.Services.AddScoped<IProductAvailablePerLineRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.ProductAvailablePerLineRepository>();
    builder.Services.AddScoped<IProductOperationRouteRepository, MetalFlowScheduler.Api.Infrastructure.Data.Repositories.ProductOperationRouteRepository>();
}

builder.Services.AddScoped<IProductionSolverService, MetalFlowScheduler.Api.Application.Services.ProductionSolverService>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // Lê a versão de um segmento da URL, ex.: /api/v2
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Formato do grupo (ex.: v2)
    options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddControllers();

// Configurar CORS para permitir requisições do frontend React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", builder =>
    {
        builder.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://jackal-infinite-penguin.ngrok-free.app")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Configurar OpenAPI e Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configuração para a versão 1
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MetalFlowScheduler.Api - V1",
        Version = "v1"
    });

    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

        var versions = apiDesc.ActionDescriptor?.EndpointMetadata
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions)
            .Select(v => $"v{v.MajorVersion}") ?? Enumerable.Empty<string>();

        return versions.Contains(docName);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MetalFlowScheduler API V1");
        options.RoutePrefix = "swagger";
        options.DefaultModelExpandDepth(-1); // Opcional: oculta o schema de modelos
    });

}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// Habilitar CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
