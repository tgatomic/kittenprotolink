using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class FlightControllerWrapper (TelemetryThresholds thresholds)
{
    private FlightComputerTelemetry? _oldFlightComputer;
    
    public FlightComputerTelemetry? BuildFlightComputerTelemetry(Vehicle vehicle)
    {
        var fc = vehicle.FlightComputer;
        var burn = fc.Burn;
        var flightComputer = new FlightComputerTelemetry
        {
            BurnMode = fc.BurnMode == FlightComputerBurnMode.Auto
                ? FlightComputerTelemetry.Types.BurnMode.Auto
                : FlightComputerTelemetry.Types.BurnMode.Manual,
            AttitudeMode = fc.AttitudeMode == FlightComputerAttitudeMode.Auto
                ? FlightComputerTelemetry.Types.AttitudeMode.Auto
                : FlightComputerTelemetry.Types.AttitudeMode.Manual,
            AttitudeFrame = fc.AttitudeFrame.ToString(),
            StabilizationActive = vehicle.IsStabilizationOn(),
            ManualThrustPulse = fc.ManualThrustMode == FlightComputerManualThrustMode.Pulse,
            BurnTimeRemaining = burn?.BurnDuration ?? 0f,
            BurnDvRemaining = burn?.DeltaVToGoCci.Length() ?? 0f
        };

        if (_oldFlightComputer == null)
        {
            _oldFlightComputer = flightComputer;
            return flightComputer;
        }
        
        var hasChanged = HasSignificantChange(_oldFlightComputer, flightComputer)? flightComputer : null;
        _oldFlightComputer = flightComputer;

        return hasChanged;
    }
    
    private static bool HasSignificantChange(FlightComputerTelemetry oldFlight, FlightComputerTelemetry newFlight)
    {
        // if (newFlight.BurnMode != oldFlight.BurnMode) return true;
        // if (newFlight.AttitudeMode != oldFlight.AttitudeMode) return true;
        // if (newFlight.AttitudeFrame != oldFlight.AttitudeFrame) return true;
        // if (newFlight.StabilizationActive != oldFlight.StabilizationActive) return true;
        // if (newFlight.ManualThrustPulse != oldFlight.ManualThrustPulse) return true;
        // if (Helpers.Diff(newFlight.BurnTimeRemaining, oldFlight.BurnTimeRemaining) > TelemetryThresholds.BurnTimeRemaining) return true;
        // if (Helpers.Diff(newFlight.BurnDvRemaining, oldFlight.BurnDvRemaining) > TelemetryThresholds.BurnDvRemaining) return true;
    
        return false;
    }
}