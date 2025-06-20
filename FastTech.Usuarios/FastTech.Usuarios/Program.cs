using System.Reflection;
using FastTech.Usuarios.Application.Interfaces;
using FastTech.Usuarios.Application.Services;
using FastTech.Usuarios.Application.Settings;
using FastTech.Usuarios.Contract.CreateClient;
using FastTech.Usuarios.Contract.CreateEmployee;
using FastTech.Usuarios.Contract.DeleteUser;
using FastTech.Usuarios.Contract.GenerateTokens;
using FastTech.Usuarios.Contract.GetUserById;
using FastTech.Usuarios.Contract.UpdateUser;
using FastTech.Usuarios.Infra.Persistense;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserCommandStore, UserCommandStore>();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection("Identity"));

builder.Services.AddValidatorsFromAssemblyContaining<CreateClientCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<DeleteUserCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<TokensCommand>();
builder.Services.AddValidatorsFromAssemblyContaining<GetUserByIdQuery>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserCommand>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Redirecionar a raiz para a UI do Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html", false);
    return Task.CompletedTask;
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 
    
    //db.Database.EnsureDeleted();   // ⚠️ CUIDADO: Apaga o banco todo
    
    // Força a criação do banco se não existir
    var bancoCriado = db.Database.EnsureCreated();
    Console.WriteLine($"Banco criado? {bancoCriado}");

    // Garante que o usuário admin será criado
    await SeedData.EnsureAdminUserAsync(scope.ServiceProvider);
}

app.Run();