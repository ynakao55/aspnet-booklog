using Microsoft.EntityFrameworkCore;
using Npgsql;
using aspnet_booklog.Data;
using aspnet_booklog.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- PostgreSQL 接続文字列（Render の DATABASE_URL 対応） ---
string? rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
// ローカル用の既定（必要なら .env などで上書き）
if (string.IsNullOrWhiteSpace(rawUrl))
{
    rawUrl = "Host=localhost;Port=5432;Database=booklog;Username=postgres;Password=postgres";
}
// Render の postgres:// 形式も Npgsql 形式に変換、SSL 必須
// 例）postgres://USER:PWD@HOST:PORT/DB → Host=HOST;Port=PORT;Database=DB;Username=USER;Password=PWD;SSL Mode=Require;Trust Server Certificate=true
var connString = ToNpgsqlConnectionString(rawUrl);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connString));

builder.Services.AddRazorPages();

var app = builder.Build();

// DB 自動マイグレーション（本番/検証で使いやすく）
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Minimal なヘルスチェック
app.MapGet("/health", () => Results.Ok("OK"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Render の $PORT にバインド（Dockerfileでも設定するが保険）
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();

static string ToNpgsqlConnectionString(string input)
{
    if (input.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        input.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        var uri = new Uri(input.Replace("postgres://", "postgresql://"));
        var userInfo = uri.UserInfo.Split(':', 2);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.Trim('/'),
            Username = userInfo[0],
            Password = userInfo.Length > 1 ? userInfo[1] : ""
        };
        // Render のマネージド PG は基本 SSL 必須
        //builder.SslMode = SslMode.Require;

// 例: builder = new NpgsqlConnectionStringBuilder { ... };
var sslEnv = (Environment.GetEnvironmentVariable("DB_SSLMODE") ?? "").Trim().ToLowerInvariant();

if (sslEnv == "disable")
{
    builder.SslMode = SslMode.Disable;
}
else if (sslEnv == "prefer")
{
    builder.SslMode = SslMode.Prefer;
}
else if (sslEnv == "require")
{
    builder.SslMode = SslMode.Require;
}
else
{
    // 指定が無い場合はホスト名で自動判定（docker-compose の db / localhost はSSLなし）
    if (builder.Host == "db" || builder.Host == "localhost" || builder.Host == "127.0.0.1")
        builder.SslMode = SslMode.Disable;   // ← DockerのPostgres向け
    else
        builder.SslMode = SslMode.Require;   // ← Render等のマネージドDB向け
}

// ついでに（SSL使う時に証明書検証を緩める必要がある環境なら）
// builder.TrustServerCertificate = true;

        builder.TrustServerCertificate = true; // 証明書検証を簡易化
        return builder.ToString();
    }
    return input; // すでに Npgsql 形式ならそのまま
}
