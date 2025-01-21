using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using Polly.Extensions.Http;


namespace SharedPolly
{
    public static class SetupPolly
    {

        //TODO
        //public static void AddPolly(IServiceCollection services)
        //public static void AddPolly(this WebApplicationBuilder builder)
        //{
        //    // Adicionando o HttpClient com Polly para Circuit Breaker e Retry
        //    services.AddHttpClient("ApiWithCircuitBreaker")
        //        .AddTransientHttpErrorPolicy(policyBuilder =>
        //            policyBuilder.WaitAndRetryAsync(3, attempt =>
        //                TimeSpan.FromSeconds(Math.Pow(2, attempt))))  // Retry com backoff exponencial
        //        .AddTransientHttpErrorPolicy(policyBuilder =>
        //            policyBuilder.CircuitBreakerAsync(3, TimeSpan.FromSeconds(10))); // 3 falhas seguidas, 10s de espera
        //}


        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddHttpClient("ApiWithCircuitBreaker")
        //        .AddPolicyHandler(GetCircuitBreakerPolicy());

        //    services.AddDbContext<ApplicationDbContext>(options =>
        //        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
        //        .AddPolicyHandler(GetRetryPolicy())); // Aplicando Retry para conexões com DB
        //}

        //private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        //{
        //    return HttpPolicyExtensions
        //        .HandleTransientHttpError()
        //        .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));  // 3 falhas seguidas, 10s de espera
        //}

        //private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        //{
        //    return (IAsyncPolicy<HttpResponseMessage>)Policy.Handle<SqlException>()
        //        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))); // Retry exponencial
        //}
    }
}
