using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using WebApi.RainIntesity.Services;

namespace RainIntensity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly JwtService _jwtService;
        public DeviceController(HttpClient httpClient, JwtService jwtService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.BaseAddress = new Uri("http://localhost:3000/");
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }



        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest model)
        {
            // Simulação de verificação de credenciais
            if (model.Username == "admin" && model.Password == "admin")
            {
                var jwtService = new JwtService(); // Cria uma instância do serviço JWT
                                                   // Credenciais válidas, então gera um token JWT
                var token = jwtService.GenerateToken(model.Username);
                return Ok(new { Token = token });
            }
            else
            {
                // Credenciais inválidas
                return Unauthorized();
            }
        }

        [HttpGet("device")]
        public async Task<IActionResult> GetDevices()
        {
            try
            {
                var response = await _httpClient.GetAsync("/devices");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                // Analisa o JSON em um JsonDocument
                using (JsonDocument doc = JsonDocument.Parse(content))
                {
                    // Inicia uma lista vazia de dispositivos filtrados
                    var filteredDevices = new List<Device>();

                    // Percorre os elementos do JSON
                    foreach (JsonElement element in doc.RootElement.EnumerateArray())
                    {
                        // Desserializa cada elemento do JSON para um objeto Device
                        var device = JsonSerializer.Deserialize<Device>(element.GetRawText());

                        // Verifica se o dispositivo tem Manufacturer igual a "PredictWeather"
                        // e contém pelo menos um comando com Operation igual a "get_rainfall_intensity"
                        if (device.Manufacturer == "PredictWeather" &&
                            device.Commands.Any(command => command.Operation == "get_rainfall_intensity"))
                        {
                            // Adiciona o dispositivo à lista de dispositivos filtrados
                            filteredDevices.Add(device);
                        }
                    }

                    // Retorna os dispositivos filtrados
                    return Ok(filteredDevices);
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Erro ao se comunicar com a API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return StatusCode(500, $"Erro ao processar a resposta da API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro inesperado: {ex.Message}");
            }
        }

        [HttpPost("device")]
        public async Task<IActionResult> CreateDevice([FromBody] Device device)
        {
            // Implemente a lógica para cadastrar um novo dispositivo na API CIoTD
            return StatusCode(501, "Funcionalidade não implementada");
        }

        [HttpGet("device/{id}")]
        public async Task<IActionResult> GetDeviceById(string id)
        {
            // Implemente a lógica para obter os detalhes de um dispositivo pelo ID
            return StatusCode(501, "Funcionalidade não implementada");
        }

        [HttpPut("device/{id}")]
        public async Task<IActionResult> UpdateDevice(string id, [FromBody] Device device)
        {
            // Implemente a lógica para atualizar os dados de um dispositivo
            return StatusCode(501, "Funcionalidade não implementada");
        }

        [HttpDelete("device/{id}")]
        public async Task<IActionResult> DeleteDevice(string id)
        {
            // Implemente a lógica para remover um dispositivo pelo ID
            return StatusCode(501, "Funcionalidade não implementada");
        }
    }

    public class DeviceResponse
    {
        public List<Device> Devices { get; set; }
    }

    public class Device
    {
        public string Identifier { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Url { get; set; }
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
        public string Operation { get; set; }
        public string Description { get; set; }
        public InnerCommand _command { get; set; }
        public string Result { get; set; }
    }

    public class InnerCommand
    {
        public string _command { get; set; }
        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}