using Microsoft.EntityFrameworkCore;
using aspnet_booklog.Data;

var builder = WebApplication.CreateBuilder(args);

var connString = BuildMySqlConnectionString();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        connString,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure());
});

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok("OK"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

static string BuildMySqlConnectionString()
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        return databaseUrl;
    }

    var host = Environment.GetEnvironmentVariable("TIDB_HOST");
    var port = Environment.GetEnvironmentVariable("TIDB_PORT") ?? "4000";
    var database = Environment.GetEnvironmentVariable("TIDB_DATABASE") ?? "booklog";
    var user = Environment.GetEnvironmentVariable("TIDB_USER");
    var password = Environment.GetEnvironmentVariable("TIDB_PASSWORD");
    var caPath = Environment.GetEnvironmentVariable("CA_PATH");

    var csb = new MySqlConnector.MySqlConnectionStringBuilder
    {
        Server = host,
        Port = uint.Parse(port),
        Database = database,
        UserID = user,
        Password = password,
        SslMode = MySqlConnector.MySqlSslMode.VerifyCA,
        Pooling = true
    };

    if (!string.IsNullOrWhiteSpace(caPath))
    {
        csb.SslCa = caPath;
    }

    return csb.ConnectionString;
}
