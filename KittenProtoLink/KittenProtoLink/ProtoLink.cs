using System.Reflection;
using System.Text.Json;
using KSA;
using StarMap.API;

namespace KittenProtoLink;

[StarMapMod]
public class ProtoLink
{
    private TelemetryServer? _telemetryServer;
    
    private long _lastSentTime;
    private const int FrequencyMs = 30;


    private SettingsMenu _settings;
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
    
    [StarMapBeforeMain]
    public void OnFullyLoaded()
    {
        _settings = new SettingsMenu();
        _telemetryBuilder = new(_settings);
        
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
