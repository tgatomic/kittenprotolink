using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class EngineWrapper
{
    private EngineTelemetry? _oldVehicle;
    
    public EngineTelemetry? BuildFlightComputerTelemetry(Vehicle vehicle)
    {
        double throttle = vehicle.GetManualThrottle();
        bool engineOn = IsEngineEnabled(vehicle);
        var fc = vehicle.FlightComputer;
        
        var newVehicle =  new EngineTelemetry
        {
            Throttle = throttle,
            MinThrottle = vehicle.GetMinThrottle(),
            EngineEnabled = engineOn,
            Thrust = fc.VehicleConfig.TotalEngineThrust * throttle,
            FuelFlow = fc.VehicleConfig.TotalEngineMassFlowRate * throttle,
        };

        if (_oldVehicle == null)
        {
            _oldVehicle = newVehicle;
            return newVehicle;
        }
        
        var hasChanged = HasSignificantChange(_oldVehicle, newVehicle)? newVehicle : null;
        _oldVehicle = newVehicle;

        return hasChanged;
    }
    
    public static bool IsEngineEnabled(Vehicle vehicle)
    {
        return vehicle.IsSet(VehicleEngine.MainIgnite, clicked: false);
    }
    
    public static void ToggleEngine(Vehicle vehicle)
    {
        bool engineOn = IsEngineEnabled(vehicle);
        vehicle.SetEnum(engineOn ? VehicleEngine.MainShutdown : VehicleEngine.MainIgnite);
    }

    private static bool HasSignificantChange(EngineTelemetry oldVehicle, EngineTelemetry newVehicle)
    {
        if (newVehicle.EngineEnabled != oldVehicle.EngineEnabled) return true;
        if (Helpers.Diff(newVehicle.FuelFlow, oldVehicle.FuelFlow) > TelemetryThresholds.FuelFlow) return true;
        if (Helpers.Diff(newVehicle.Throttle, oldVehicle.Throttle) > TelemetryThresholds.Throttle) return true;
        if (Helpers.Diff(newVehicle.MinThrottle, oldVehicle.MinThrottle) > TelemetryThresholds.Throttle) return true;
        if (Helpers.Diff(newVehicle.Thrust, oldVehicle.Thrust) > TelemetryThresholds.Thrust) return true;
        
        return false;
    }
}