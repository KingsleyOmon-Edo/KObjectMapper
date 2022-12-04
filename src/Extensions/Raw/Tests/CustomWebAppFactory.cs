//namespace Extensions.Tests;

//public class CustomWebAppFactory<TStartup> : WebApplicationFactory<TStartup>
//  where TStartup : class
//{
//    protected override void ConfigureWebHost(IWebHostBuilder builder)
//    {
//        builder.ConfigureServices(services =>
//        {
//            var descriptor =
//          services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

//            services.Remove(descriptor);

//            services.AddDbContext<ApplicationDbContext>(options =>
//          options.UseInMemoryDatabase("InMemoryDbForTesting"));

//            var sp = services.BuildServiceProvider();

//            using (var scope = sp.CreateScope())
//            {
//                var scopedServices = scope.ServiceProvider;
//                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
//                var logger = scopedServices.GetRequiredService<ILogger<CustomWebAppFactory<TStartup>>>();

//                try
//                {
//                    db.Database.EnsureCreated();
//                }
//                catch (Exception ex)
//                {
//                    // Log this event.

//                }
//            }

//        });
//    }
//}



//public class ApplicationDbContext : DbContext
//{
//    public ApplicationDbContext(DbContextOptions options) : base(options)
//    {

//    }
//}
