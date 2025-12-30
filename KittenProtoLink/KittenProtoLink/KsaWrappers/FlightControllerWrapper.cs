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
        
        if (!thresholds.FlightComputer.BurnTimeRemaining.Active)  flightComputer.BurnTimeRemaining = 0;
        if (!thresholds.FlightComputer.BurnDvRemaining.Active)    flightComputer.BurnDvRemaining = 0;
        
        if (_oldFlightComputer == null)
        {
            _oldFlightComputer = flightComputer;
            return flightComputer;
        }
        
        var hasChanged = HasSignificantChange(_oldFlightComputer, flightComputer)? flightComputer : null;
        _oldFlightComputer = flightComputer;

        return hasChanged;
    }
    
    private bool HasSignificantChange(FlightComputerTelemetry oldFlight, FlightComputerTelemetry newFlight)
    {
        if (newFlight.BurnMode != oldFlight.BurnMode) return true;
        if (newFlight.AttitudeMode != oldFlight.AttitudeMode) return true;
        if (newFlight.AttitudeFrame != oldFlight.AttitudeFrame) return true;
        if (newFlight.StabilizationActive != oldFlight.StabilizationActive) return true;
        if (newFlight.ManualThrustPulse != oldFlight.ManualThrustPulse) return true;
        if (thresholds.FlightComputer.BurnTimeRemaining.Active &&
            Helpers.Diff(newFlight.BurnTimeRemaining, oldFlight.BurnTimeRemaining) > thresholds.FlightComputer.BurnTimeRemaining.Value) return true;
        if (thresholds.FlightComputer.BurnDvRemaining.Active &&
            Helpers.Diff(newFlight.BurnDvRemaining, oldFlight.BurnDvRemaining) > thresholds.FlightComputer.BurnDvRemaining.Value) return true;
    
        return false;
    }
}
