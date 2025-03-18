namespace Statistics.Commandline;

internal class Program
{
    static void Main(string[] args)
    {
        var program = new Program();

        program.Run(args);
    }

    private void Run(string[] args)
    {
        var startup = new Startup();

        IApplicationBuilder builder = CreateBuilder(args);
        startup.SetupApplication(builder)
            .Configure(host => host.ConfigureServices(collection => Startup.SetupServices(collection)));
    }

    private IApplicationBuilder CreateBuilder(string[] args)
    {
        throw new NotImplementedException();
    }
}
