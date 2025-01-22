using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#if DEBUG
    builder.Configuration.AddJsonFile("ocelot.dev.json", optional: false, reloadOnChange: true);
#else
    builder.Configuration.AddJsonFile("ocelot.prod.json", optional: false, reloadOnChange: true);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(80); // Porta do container
    });
#endif


builder.Services.AddOcelot();

builder.Services.AddControllers();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.UseOcelot();

app.Run();

