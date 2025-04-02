using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Statistics.Shared.Abstraction.Enum.Persistence;
using Statistics.Shared.Models.Entity;
using Statistics.Shared.Persistence.Configuration;
using Statistics.Shared.Persistence.Core;

namespace Statistics.Shared.Persistence;

public class StatisticsDatabaseContext : BaseDatabaseContext
{
    public StatisticsDatabaseContext(DbContextOptions options) : base(options)
    {
        //Console.WriteLine($"Completed Construction of Database Context. - {JsonConvert.SerializeObject(options)}");
        Console.WriteLine($"Completed Construction of Database Context.");
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine("Building Database Model...");
        base.OnModelCreating(modelBuilder);

        const DatabaseType databaseType = DatabaseType.POSTGRESS;

        modelBuilder.ApplyConfiguration(new ArtificialIntelligenceConfiguration(databaseType));
        modelBuilder.ApplyConfiguration(new PromptConfiguration(databaseType));
        modelBuilder.ApplyConfiguration(new ResponseConfiguration(databaseType));
        modelBuilder.ApplyConfiguration(new KeywordConfiguration(databaseType));
    }

    public DbSet<ArtificialIntelligence> ArtificialIntelligences { get; set; }
    public DbSet<Prompt> Prompts { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
}
