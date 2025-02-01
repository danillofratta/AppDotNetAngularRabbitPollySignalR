using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("CorsPolicy",
//        builder =>
//        {
//            builder
//                //.WithOrigins("http://localhost:4200") // sua URL do Angular
//                .AllowAnyOrigin()
//                .AllowAnyMethod()
//                .AllowAnyHeader()
//                .AllowCredentials(); // se necessário
//        });
//});


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

app.UseCors(cors => cors
.AllowAnyMethod()
.AllowAnyHeader()
.AllowAnyOrigin()
);

app.UseCors((g) => g.AllowAnyOrigin());
app.UseCors((g) => g.AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseWebSockets();

//app.UseCors("CorsPolicy");
await app.UseOcelot();

app.Run();

