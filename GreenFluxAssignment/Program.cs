using GreenFluxAssignment.Controllers;
using GreenFluxAssignment.Data;
using GreenFluxAssignment.Exceptions;
using GreenFluxAssignment.Repositories;
using GreenFluxAssignment.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// add problem details for better problem responses
builder.Services.AddProblemDetails();

// add custom exception handler
builder.Services.AddExceptionHandler<ProblemExceptionHandler>();

// Add services to the container.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite("Data Source=app.db");
});
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IChargeStationRepository, ChargeStationRepository>();
builder.Services.AddScoped<IConnectorRepository, ConnectorRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IChargeStationService, ChargeStationService>();
builder.Services.AddScoped<IConnectorService, ConnectorService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
