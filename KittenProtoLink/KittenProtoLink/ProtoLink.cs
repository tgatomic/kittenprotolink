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
    
    private const string ThreasholdsJsonFile = "Thresholds.json";

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
        var dllPath = Assembly.GetExecutingAssembly().Location;
        var dllDir  = Path.GetDirectoryName(dllPath)!;
        var filePath = Path.Combine(dllDir, ThreasholdsJsonFile);
        
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
            
            Console.WriteLine("Thresholds loaded");
            Console.WriteLine(JsonSerializer.Serialize(_thresholds));
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
