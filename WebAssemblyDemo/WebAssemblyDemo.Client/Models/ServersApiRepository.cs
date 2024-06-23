using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace WebAssemblyDemo.Client.Models;

public class ServersApiRepository : IServersRepository
{
    private const string apiName = "ServersApi";
    private readonly IHttpClientFactory _httpClientFactory;

    public ServersApiRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Server?> GetServerByIdAsync(int id)
    {
        var httpClient = _httpClientFactory.CreateClient(apiName);

        var response = await httpClient.GetAsync($"servers/{id}.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Server>(content);
    }

    public async Task<List<Server>> GetServersAsync()
    {
        var httpClient = _httpClientFactory.CreateClient(apiName);
        var response = await httpClient.GetAsync("servers.json");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(content) && content != "null")
        {
            return JsonConvert.DeserializeObject<List<Server>>(content) ?? new List<Server>();
        }
        return new List<Server>();
    }

    public async Task AddServerAsync(Server server)
    {
        server.ServerId = await GetNextServerIdAsync();

        var httpClient = _httpClientFactory.CreateClient(apiName);
        var content = new StringContent(JsonConvert.SerializeObject(server), Encoding.UTF8, "application/json");

        var response = await httpClient.PutAsync($"servers/{server.ServerId}.json", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateServerAsync(int serverId, Server server)
    {
        if (serverId != server.ServerId) return;

        var httpClient = _httpClientFactory.CreateClient(apiName);
        var content = new StringContent(JsonConvert.SerializeObject(server), Encoding.UTF8, "application/json");

        var response = await httpClient.PatchAsync($"servers/{server.ServerId}.json", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteServerAsync(int serverId)
    {
        var httpClient = _httpClientFactory.CreateClient(apiName);
        await httpClient.DeleteAsync($"servers/{serverId}.json");
    }

    private async Task<int> GetNextServerIdAsync()
    {
        var servers = await GetServersAsync();
        if(servers is not null && servers.Any())
            return servers.Where(x => x is not null).Max(x => x.ServerId) + 1;

        return 1;
    }
}
