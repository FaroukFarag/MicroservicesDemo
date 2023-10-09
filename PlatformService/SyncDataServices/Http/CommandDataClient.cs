using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace PlatformService.SyncDataServices.Http
{
    public class CommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                "application/json");

            Console.WriteLine($"Connection Is = {_configuration["CommandService"]}");

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}/api/c/Platforms", httpContent);

            if(response.IsSuccessStatusCode)
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            else
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
        }
    }
}
