using KSA;
using StarMap.API;

namespace KittenProtoLink;

public class ProtoLink : IStarMapMod, IStarMapOnUi
{
    public bool ImmediateUnload => false;

    private TelemetryServer? _telemetryServer;
    
    private long _lastSentTime;
    private const int FrequencyMs = 30;

    private readonly TelemetryBuilder _telemetryBuilder = new();
    private readonly TranslationControl _translationControl = new();

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

    public void OnBeforeUi(double dt)
    {
    }

    public void OnFullyLoaded()
    {
        _telemetryServer = new TelemetryServer();
        _telemetryServer.CommandReceived += _translationControl.HandleCommand;
        _telemetryServer.Start();

        Patcher.Patch();
    }

    public void OnImmediatLoad()
    {
    }

    public void Unload()
    {
        _telemetryServer?.Dispose();
        _telemetryServer = null;
        Patcher.Unload();
    }
}
