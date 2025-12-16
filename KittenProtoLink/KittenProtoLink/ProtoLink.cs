using System.Reflection;
using System.Text.Json;
using KittenProtoLink.KsaWrappers;
using KSA;
using StarMap.API;

namespace KittenProtoLink;

[StarMapMod]
public class ProtoLink
{
    private TelemetryServer? _telemetryServer;
    
    private long _lastSentTime;
    private const int FrequencyMs = 30;

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

    [StarMapBeforeGui]
    public void OnBeforeUi(double dt)
    {
    }

    [StarMapBeforeMain]
    public void OnFullyLoaded()
    {
        _telemetryServer = new TelemetryServer();
        _telemetryServer.CommandReceived += _translationControl.HandleCommand;
        _telemetryServer.Start();
        
        var dllPath = Assembly.GetExecutingAssembly().Location;
        var dllDir  = Path.GetDirectoryName(dllPath)!;
        var filePath = Path.Combine(dllDir, "Thresholds.json");
        
        if (File.Exists(filePath))
        {
            var thresholdsRaw = File.ReadAllText("Thresholds.json");
            _thresholds = JsonSerializer.Deserialize<TelemetryThresholds>(thresholdsRaw) ?? new TelemetryThresholds();
            
            Console.WriteLine("Thresholds loaded");
        }
        else
        {
            throw new Exception("Thresholds file not found");
        }

        _telemetryBuilder = new(_thresholds);

        Patcher.Patch();
    }

    [StarMapImmediateLoad]
    public void OnImmediatLoad()
    {
    }

    [StarMapUnload]
    public void Unload()
    {
        _telemetryServer?.Dispose();
        _telemetryServer = null;
        Patcher.Unload();
    }
}
