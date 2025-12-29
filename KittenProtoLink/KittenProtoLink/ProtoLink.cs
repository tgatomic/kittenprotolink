using System.Reflection;
using System.Text.Json;
using Brutal.ImGuiApi;
using KSA;
using ModMenu;
using StarMap.API;

namespace KittenProtoLink;

[StarMapMod]
public class ProtoLink
{
    private TelemetryServer? _telemetryServer;
    
    private long _lastSentTime;
    private const int FrequencyMs = 30;
    private const string ThresholdsJsonFile = "Thresholds.json";

    private TelemetryThresholds _thresholds;
    private TelemetryBuilder _telemetryBuilder;
    private readonly TranslationControl _translationControl = new();
    
    [StarMapAfterGui]
    public void OnAfterUi(double dt)
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > _lastSentTime + FrequencyMs)
        {
            _lastSentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            var vehicle = Program.ControlledVehicle;
            if (vehicle == null) return;
            
            var envelope = _telemetryBuilder.BuildTelemetryEnvelope(vehicle);
            if (envelope != null)
                _telemetryServer?.Broadcast(envelope);
        }
    }
    
    [ModMenuEntry("ModMenu KittenProtoLink")]
    public void DrawSubMenuEntry()
    {
        Console.WriteLine("DISCOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO");
        ImGui.Text("The test worked!");
        ImGui.Render();
    }
    
    [StarMapBeforeMain]
    public void OnFullyLoaded()
    {
        var dllPath = Assembly.GetExecutingAssembly().Location;
        var dllDir  = Path.GetDirectoryName(dllPath)!;
        var filePath = Path.Combine(dllDir, ThresholdsJsonFile);
        
        if (File.Exists(filePath))
        {
            var thresholdsRaw = File.ReadAllText(filePath);
            try
            {
                _thresholds = JsonSerializer.Deserialize<TelemetryThresholds>(thresholdsRaw)!;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to decode Thresholds.json: " + e.Message);
                return;
            }
        }
        else
        {
            throw new Exception("Thresholds file not found");
        }

        _telemetryBuilder = new(_thresholds);
        
        _telemetryServer = new TelemetryServer();
        _telemetryServer.CommandReceived += _translationControl.HandleCommand;
        _telemetryServer.Start();

        Patcher.Patch();
    }

    [StarMapUnload]
    public void Unload()
    {
        _telemetryServer?.Dispose();
        _telemetryServer = null;
        Patcher.Unload();
    }
}
