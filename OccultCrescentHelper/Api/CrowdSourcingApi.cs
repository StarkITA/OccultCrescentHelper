using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Text.Json;
using ECommons.DalamudServices;

namespace OccultCrescentHelper.Api;

public class CrowdSourcingApi
{
    private readonly HttpClient client = new HttpClient();

    private Plugin plugin;

    public CrowdSourcingApi(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public CrowdSourcingConfig config
    {
        get => plugin.config.CrowdSourcingConfig;
    }

    public void SendMonsterSpawn(MonsterPayload payload)
    {
        if (!config.ShareMonsterSpawnData)
        {
            return;
        }

        var url = "https://api.oc.ohkannaduh.com/monster_spawn";

        if (config.SharedMonsterSpawns.Contains(payload))
        {
            return;
        }

        config.SharedMonsterSpawns.Add(payload);
        plugin.config.Save();

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("API_KEY"));

        try
        {
            var response = client.PostAsync(url, content).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Svc.Log.Info("Data sent successfully.");
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Svc.Log.Info($"Response: {responseBody}");
            }
            else
            {
                Svc.Log.Error($"Failed to send data. Status code: {response.StatusCode}");
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Svc.Log.Error($"Error response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error($"Failed to send request: {ex.Message}");
        }
    }

    public void SendObjectPosition(ObjectPositionPayload payload)
    {
        if (!config.ShareObjectPositionData)
        {
            return;
        }

        var url = "https://api.oc.ohkannaduh.com/object_position";

        if (config.SharedObjectPosition.Contains(payload))
        {
            return;
        }

        config.SharedObjectPosition.Add(payload);
        plugin.config.Save();

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("API_KEY"));

        try
        {
            var response = client.PostAsync(url, content).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Svc.Log.Info("Data sent successfully.");
                var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Svc.Log.Info($"Response: {responseBody}");
            }
            else
            {
                Svc.Log.Error($"Failed to send data. Status code: {response.StatusCode}");
                var errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Svc.Log.Error($"Error response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error($"Failed to send request: {ex.Message}");
        }
    }

    public void SendTreasure(Vector3 position, uint? modelId)
    {
        ObjectPositionPayload payload = new ObjectPositionPayload
        {
            type = ObjectType.Treasure,
            position = new Position
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
            },
            model_id = modelId,
        };

        SendObjectPosition(payload);
    }

    public void SendCarrot(Vector3 position)
    {
        ObjectPositionPayload payload = new ObjectPositionPayload
        {
            type = ObjectType.Carrot,
            position = new Position
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
            },
        };

        SendObjectPosition(payload);
    }
}
