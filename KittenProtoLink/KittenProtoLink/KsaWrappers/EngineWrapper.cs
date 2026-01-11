using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class EngineWrapper (SettingsMenu settings)
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
            Thrust = fc.VehicleConfig.TotalEngineVacuumThrust * throttle,
            FuelFlow = fc.VehicleConfig.TotalEngineVacuumMassFlowRate * throttle,
        };

        if (!settings.Thresholds.Engine.FuelFlow.Active) newVehicle.FuelFlow = 0;
        if (!settings.Thresholds.Engine.Throttle.Active)
        {
            newVehicle.Throttle = 0;
            newVehicle.MinThrottle = 0;
        }
        if (!settings.Thresholds.Engine.Thrust.Active) newVehicle.Thrust = 0;

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

    private bool HasSignificantChange(EngineTelemetry oldVehicle, EngineTelemetry newVehicle)
    {
        if (newVehicle.EngineEnabled != oldVehicle.EngineEnabled) return true;
        if (settings.Thresholds.Engine.FuelFlow.Active &&
            Helpers.Diff(newVehicle.FuelFlow, oldVehicle.FuelFlow) > settings.Thresholds.Engine.FuelFlow.Value) return true;
        if (settings.Thresholds.Engine.Throttle.Active &&
            Helpers.Diff(newVehicle.Throttle, oldVehicle.Throttle) > settings.Thresholds.Engine.Throttle.Value) return true;
        if (settings.Thresholds.Engine.Throttle.Active &&
            Helpers.Diff(newVehicle.MinThrottle, oldVehicle.MinThrottle) > settings.Thresholds.Engine.Throttle.Value) return true;
        if (settings.Thresholds.Engine.Thrust.Active &&
            Helpers.Diff(newVehicle.Thrust, oldVehicle.Thrust) > settings.Thresholds.Engine.Thrust.Value) return true;
        
        return false;
    }
}
