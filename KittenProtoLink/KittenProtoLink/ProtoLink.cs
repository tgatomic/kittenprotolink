using KSA;
using StarMap.API;

namespace KittenProtoLink;

[StarMapMod]
public class ProtoLink
{
    private TelemetryServer? _telemetryServer;
    
    private long _lastSentTime;
    private const int FrequencyMs = 30;

    private readonly TelemetryBuilder _telemetryBuilder = new();
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
