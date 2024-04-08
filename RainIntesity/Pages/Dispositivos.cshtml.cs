using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RainIntesity.Pages
{
    //[Authorize]
    public class DispositivosModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DispositivosModel(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("https://localhost:7135/"); // Defina o URL base da sua API aqui
        }

        public List<Device> Dispositivos { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/Device/device");
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                Dispositivos = JsonSerializer.Deserialize<List<Device>>(content);
            }
            catch (HttpRequestException ex)
            {
                ViewData["ErrorMessage"] = $"Erro na requisição HTTP: {ex.Message}";
            }
            catch (JsonException ex)
            {
                ViewData["ErrorMessage"] = $"Erro na desserialização JSON: {ex.Message}";
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = $"Erro inesperado: {ex.Message}";
            }
        }

        public class Device
        {
            public string identifier { get; set; }
            public string description { get; set; }
            public string manufacturer { get; set; }
            public string url { get; set; }
            public List<CommandDescription> Commands { get; set; }
        }

        public class CommandDescription
        {
            public string Operation { get; set; }
            public string Description { get; set; }
            public Command Command { get; set; }
            public string Result { get; set; }
        }

        public class Command
        {
            public string Commands { get; set; }
            public List<Parameter> Parameters { get; set; }
        }

        public class Parameter
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}