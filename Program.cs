using MediConnect.Web.Models;
using MediConnect.Web.Services;

var builder = WebApplication.CreateBuilder(args);

//  Register MVC controllers and views
builder.Services.AddControllersWithViews();

// Authorization (required for app.UseAuthorization())
builder.Services.AddAuthorization();

//  Register HTTP client for NPI API
builder.Services.AddHttpClient<MediConnect.Web.Services.NpiClient>();
builder.Services.AddHttpClient("npi", c =>
{
    c.BaseAddress = new Uri("https://npiregistry.cms.hhs.gov/");
    c.Timeout = TimeSpan.FromSeconds(10);
});

// Register application-wide singletons (in-memory persistence)
builder.Services.AddSingleton<InMemoryRepository<Patient>>();
builder.Services.AddSingleton<InMemoryRepository<Provider>>();
builder.Services.AddSingleton<InMemoryRepository<Appointment>>();
builder.Services.AddSingleton<JsonBackedStore>();

// Register custom services
builder.Services.AddSingleton<NpiClient>();

var app = builder.Build();

//  Load persisted data on startup
using (var scope = app.Services.CreateScope())
{
    var store = scope.ServiceProvider.GetRequiredService<JsonBackedStore>();
    store.Load();
}

// Save data on shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var store = scope.ServiceProvider.GetRequiredService<JsonBackedStore>();
    store.Save();
});

//  Error handling & HTTPS configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//  Middleware order (recommended best practice)
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

//  Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

//  Run the application
app.Run();
