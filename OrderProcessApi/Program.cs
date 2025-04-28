using Microsoft.EntityFrameworkCore;
using FluentValidation;
using OrderProcess.Data.DataExtentions;
using OrderProcess.Data.Repositories;
using OrderProcess.Infrastructure;
using OrderProcessApi.OrderProcessing.Validations;
using OrderProcess.Application.Services.Commands.CreateOrder;
using System.Reflection;
using OrderProcess.Application.Services.Queries.GetOrderById;
using OrderProcess.Application.Services;
using OrderProcess.Infrastructure.Configurations;
using OrderProcess.Infrastructure.Configurations.OrderProcessApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCustomSwagger();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetOrderByIdQueryHandler).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderCommandValidator>();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<OrderMapperProfile>();
}, typeof(OrderMapperProfile).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Processing API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();