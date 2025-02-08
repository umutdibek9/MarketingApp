
using MassTransit;
using Shared;
using Microsoft.Extensions.Hosting;
using Payment.API.Consumers;
using Microsoft.EntityFrameworkCore;
using Payment.API.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedEventConsumer>();
    x.AddConsumer<CargoArrivedEventConsumer>();
    x.AddConsumer<CargoFailedEventConsumer>();
  
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockReservedEventQueueName, e =>
        {
            e.ConfigureConsumer<StockReservedEventConsumer>(context);
        });

        cfg.ReceiveEndpoint(RabbitMQSettingsConst.CargoArrivedEventQueueName, e =>
        {
            e.ConfigureConsumer<CargoArrivedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.CargoFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<CargoFailedEventConsumer>(context);
        });
    });
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("PaymentDb");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Payment.Any())
    {
        context.Payment.Add(new Payment.API.Models.Payment() { id = 100, Balance = 3000 });
        context.SaveChanges();
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
