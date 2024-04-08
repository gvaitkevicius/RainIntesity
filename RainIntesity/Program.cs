using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicione o servi�o JwtService como singleton
builder.Services.AddSingleton<JwtService>(new JwtService("TokenTesteParaDesafioBackEnd_GiovaniVaitkevicius"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TokenTesteParaDesafioBackEnd_GiovaniVaitkevicius")),
            ClockSkew = TimeSpan.Zero
        };
    });

// Adicione os servi�os de p�ginas Razor
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Login", "");
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configurar o pipeline de solicita��es HTTP
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Habilitar autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();