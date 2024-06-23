using Microsoft.EntityFrameworkCore;
using ServerManagement.Data;

namespace ServerManagement.Models;

public class ServerEFCoreRepository : IServerEFCoreRepository
{
    private readonly IDbContextFactory<ServerManagementContext> _contextFactory;

    public ServerEFCoreRepository(IDbContextFactory<ServerManagementContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public void AddServer(Server server)
    {
        using var db = _contextFactory.CreateDbContext();
        db.Servers.Add(server);
        db.SaveChanges();
    }

    public List<Server> GetServers()
    {
        using var db = _contextFactory.CreateDbContext();
        return db.Servers.ToList();
    }

    public List<Server> GetServersByCity(string cityName)
    {
        using var db = _contextFactory.CreateDbContext();
        return db.Servers.Where(x => x.City != null && x.City.ToLower().IndexOf(cityName.ToLower()) >= 0).ToList();
    }

    public Server? GetServerById(int id)
    {
        using var db = _contextFactory.CreateDbContext();
        var server = db.Servers.Find(id);
        if (server is not null) return server;
        return new Server();
    }

    public void UpdateServer(int serverId, Server server)
    {
        if (server == null) throw new ArgumentNullException(nameof(server));
        if (serverId != server.ServerId) return;

        using var db = _contextFactory.CreateDbContext();
        var serverToUpdate = db.Servers.Find(serverId);
        if (serverToUpdate != null)
        {
            serverToUpdate.IsOnline = server.IsOnline;
            serverToUpdate.Name = server.Name;
            serverToUpdate.City = server.City;

            db.SaveChanges();
        }
    }

    public void DeleteServer(int serverId)
    {
        using var db = _contextFactory.CreateDbContext();
        var server = db.Servers.Find(serverId);
        if (server is null) return;

        db.Servers.Remove(server);
        db.SaveChanges();
    }

    public List<Server> SearchServers(string serverFilter)
    {
        using var db = _contextFactory.CreateDbContext();
        return db.Servers.Where(s => s.Name != null && s.Name.ToLower().IndexOf(serverFilter.ToLower()) >= 0).ToList();
    }
}
