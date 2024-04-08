using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

public class LoginModel : PageModel
{
    private readonly HttpClient _httpClient;

    public LoginModel(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri("https://localhost:7135/"); // Defina o URL base da sua API aqui
    }

    public async Task<IActionResult> OnPostAsync(string username, string password)
    {
        // Preparar os dados do login
        var loginData = new
        {
            Username = username,
            Password = password
        };

        try
        {
            // Enviar solicitação POST para fazer login
            var response = await _httpClient.PostAsJsonAsync("/api/Device/login", loginData);

            // Verificar se a resposta é um sucesso (código 200)
            if (response.IsSuccessStatusCode)
            {
                // Ler o token JWT da resposta
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Armazenar o token JWT para uso futuro, por exemplo, em cookies ou cabeçalhos de autorização
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.Token);

                // Redirecionar para a página de dispositivos
                return RedirectToPage("/Dispositivos");
            }
            else
            {
                // Se o login falhar, exibir uma mensagem de erro baseada no código de status HTTP
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    TempData["Message"] = "Credenciais inválidas. Tente novamente.";
                }
                else
                {
                    TempData["Message"] = $"Erro ao fazer login: {response.StatusCode}";
                }
                return Page();
            }
        }
        catch (HttpRequestException ex)
        {
            // Se houver um erro de comunicação com a API, exibir uma mensagem de erro
            TempData["Message"] = $"Erro ao se comunicar com a API: {ex.Message}";
            return Page();
        }
    }

    public class TokenResponse
    {
        public string Token { get; set; }
    }

}

