using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommons.DalamudServices;


namespace OccultCrescentHelper.Modules.CrowdSourcing.Api;

public class CrowdSourcingApi
{
    private readonly HttpClient client = new HttpClient();

    private Plugin plugin;

    public CrowdSourcingApi(Plugin plugin)
    {
        this.plugin = plugin;
    }

    public CrowdSourcingConfig config {
        get => plugin.config.CrowdSourcingConfig;
    }

    public async Task SendObjectPosition(ObjectPositionPayload payload)
    {
        if (!config.ShareObjectPositionData || config.SharedObjectPosition.Contains(payload))
            return;

        config.SharedObjectPosition.Add(payload);
        plugin.config.Save();

        var url = "https://api.oc.ohkannaduh.com/object_position";
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("API_KEY"));

        try
        {
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                Svc.Log.Info("Data sent successfully.");
                var responseBody = await response.Content.ReadAsStringAsync();
                Svc.Log.Info($"Response: {responseBody}");
            }
            else
            {
                Svc.Log.Error($"Failed to send data. Status code: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                Svc.Log.Error($"Error response: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Svc.Log.Error($"Failed to send request: {ex.Message}");
        }
    }

    public async Task SendTreasure(Vector3 position, uint? modelId)
    {
        ObjectPositionPayload payload = new ObjectPositionPayload {
            type = ObjectType.Treasure,
            position = new Position {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
            },
            model_id = modelId,
        };

        await SendObjectPosition(payload);
    }

    public async Task SendCarrot(Vector3 position)
    {
        ObjectPositionPayload payload = new ObjectPositionPayload {
            type = ObjectType.Carrot,
            position = new Position {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
            },
        };

        await SendObjectPosition(payload);
    }
}
