using System;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Text.Json;
using DotNetEnv;
using ECommons.DalamudServices;

namespace OccultCrescentHelper;

class Api
{
    public enum DataType
    {
        BronzeTreasure = 1,
        SilverTreasure = 2,
        Carrot = 3,
    }

    public struct Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    private struct DataPayload
    {
        public DataType type { get; set; }
        public Position position { get; set; }
    }

    private static readonly HttpClient client = new HttpClient();

    public static void SendData(DataType type, Vector3 position)
    {
        var url = "https://api.oc.ohkannaduh.com/data";

        DataPayload payload = new DataPayload
        {
            type = type,
            position = new Position
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
            },
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        client.DefaultRequestHeaders.Clear();

        client.DefaultRequestHeaders.Add(
            "x-api-key",
            Environment.GetEnvironmentVariable("API_KEY")
        );

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

    public static void SendBronzeTreasure(Vector3 position, Config config)
    {
        if (!config.BronzeTreasureLocations.Contains(position))
        {
            SendData(DataType.BronzeTreasure, position);
            config.BronzeTreasureLocations.Add(position);
            config.Save();
        }
    }

    public static void SendSilverTreasure(Vector3 position, Config config)
    {
        if (!config.SilverTreasureLocations.Contains(position))
        {
            SendData(DataType.SilverTreasure, position);
            config.SilverTreasureLocations.Add(position);
            config.Save();
        }
    }

    public static void SendCarrot(Vector3 position, Config config)
    {
        if (!config.CarrotLocations.Contains(position))
        {
            SendData(DataType.Carrot, position);
            config.CarrotLocations.Add(position);
            config.Save();
        }
    }
}
