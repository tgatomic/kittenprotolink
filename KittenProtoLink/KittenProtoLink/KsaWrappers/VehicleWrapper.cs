using System.Security.Cryptography;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class VehicleWrapper (TelemetryThresholds thresholds)
{
    private readonly EngineWrapper _engineWrapper = new(thresholds);
    private readonly MassWrapper _massWrapper = new();

    public VehicleTelemetry? BuildVehicleTelemetry(Vehicle vehicle)
    {
        var newVehicle = new VehicleTelemetry();
        
        var mass = _massWrapper.BuildMassTelemetry(vehicle);
        if  (mass != null)
            newVehicle.Mass = mass;

        var engine = _engineWrapper.BuildFlightComputerTelemetry(vehicle);
        if (engine != null)
            newVehicle.Engine = engine;
        
        return mass != null || engine != null ? newVehicle : null;
    }
}