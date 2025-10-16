using Microsoft.EntityFrameworkCore;
using Npgsql;
using aspnet_booklog.Data;
using aspnet_booklog.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// --- PostgreSQL �ڑ�������iRender �� DATABASE_URL �Ή��j ---
string? rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
// ���[�J���p�̊���i�K�v�Ȃ� .env �Ȃǂŏ㏑���j
if (string.IsNullOrWhiteSpace(rawUrl))
{
    rawUrl = "Host=localhost;Port=5432;Database=booklog;Username=postgres;Password=postgres";
}
// Render �� postgres:// �`���� Npgsql �`���ɕϊ��ASSL �K�{
// ��jpostgres://USER:PWD@HOST:PORT/DB �� Host=HOST;Port=PORT;Database=DB;Username=USER;Password=PWD;SSL Mode=Require;Trust Server Certificate=true
var connString = ToNpgsqlConnectionString(rawUrl);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connString));

builder.Services.AddRazorPages();

var app = builder.Build();

// DB �����}�C�O���[�V�����i�{��/���؂Ŏg���₷���j
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Minimal �ȃw���X�`�F�b�N
app.MapGet("/health", () => Results.Ok("OK"));

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

// Render �� $PORT �Ƀo�C���h�iDockerfile�ł��ݒ肷�邪�ی��j
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
        // Render �̃}�l�[�W�h PG �͊�{ SSL �K�{
        builder.SslMode = SslMode.Require;
        builder.TrustServerCertificate = true; // �ؖ������؂��ȈՉ�
        return builder.ToString();
    }
    return input; // ���ł� Npgsql �`���Ȃ炻�̂܂�
}
