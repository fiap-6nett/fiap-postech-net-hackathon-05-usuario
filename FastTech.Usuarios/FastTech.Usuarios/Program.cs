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
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

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

app.Run();