using Statistics.Uno.Endpoints;
using Refit;

namespace Statistics.Uno.Startup;

public class UnoRefitStartupModule<TEndpoint> : IUnoStartupModule where TEndpoint : IRefitEndpoint
{
    private readonly string baseAddress;

    public UnoRefitStartupModule(string baseAddress)
    {
        this.baseAddress = baseAddress;
    }

    /// <inheritdoc />
    public void ConfigureServices(IServiceCollection services)
    {
        var api = RestService.For<TEndpoint>(baseAddress);

        services.AddSingleton(typeof(TEndpoint), api);
    }

    /// <inheritdoc />
    public void ConfigureApplication(IApplicationBuilder app)
    {
        //app.Configure(hostBuilder => { hostBuilder.UseHttp(ConfigureRefit); });
    }

    private void ConfigureRefit(HostBuilderContext context, IServiceCollection services)
    {
        //services.AddRefitClient<TEndpoint>().ConfigureHttpClient(httpClient =>
        //{
        //    httpClient.BaseAddress = new Uri(baseAddress);
        //});
    }
}
