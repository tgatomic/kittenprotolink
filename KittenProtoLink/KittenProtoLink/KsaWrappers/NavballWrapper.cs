using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class NavballWrapper (TelemetryThresholds thresholds)
{
    private NavballTelemetry? _last;

    public NavballTelemetry? BuildNavballTelemetry(Vehicle vehicle) {
        var nav = vehicle.NavBallData;
        var telemetry = new NavballTelemetry {
            Frame = nav.Frame switch
            {
                VehicleReferenceFrame.EclBody  => NavballTelemetry.Types.Frame.Eclbody,
                VehicleReferenceFrame.EnuBody  => NavballTelemetry.Types.Frame.Enubody,
                VehicleReferenceFrame.Lvlh     => NavballTelemetry.Types.Frame.Vlfbody,
                VehicleReferenceFrame.VlfBody  => NavballTelemetry.Types.Frame.Vlfbody,
                VehicleReferenceFrame.BurnBody => NavballTelemetry.Types.Frame.Burn,
                VehicleReferenceFrame.Dock => NavballTelemetry.Types.Frame.Tgt,
                _                     => NavballTelemetry.Types.Frame.Unknown,
            },
            NavballToBody = Helpers.ToQuaterniond(nav.Navball2Body),
            AttitudeAnglesDeg = Helpers.ToVector3d(nav.AttitudeAngles),
            AttitudeRatesRad = Helpers.ToVector3d(nav.AttitudeRates)
        };
        
        if (!thresholds.Orbit.Apoapsis.Active)     telemetry.NavballToBody = default;
        if (!thresholds.Orbit.Periapsis.Active)    telemetry.AttitudeAnglesDeg = default;
        if (!thresholds.Orbit.Inclination.Active)  telemetry.AttitudeRatesRad = default;

        if (_last == null || HasSignificantChange(_last, telemetry)) {
            _last = telemetry;
            return telemetry;
        }

        return null;
    }
    
    private bool HasSignificantChange(NavballTelemetry oldOrbit, NavballTelemetry newOrbit)
    {
        if (oldOrbit.Frame != newOrbit.Frame) return true;
        if (thresholds.Orbit.Apoapsis.Active &&
            Helpers.Diff(newOrbit.NavballToBody, oldOrbit.NavballToBody) > thresholds.Orbit.Apoapsis.Value) return true;
        if (thresholds.Orbit.Periapsis.Active &&
            Helpers.Diff(newOrbit.AttitudeAnglesDeg, oldOrbit.AttitudeAnglesDeg) > thresholds.Orbit.Periapsis.Value) return true;
        if (thresholds.Orbit.Inclination.Active &&
            Helpers.Diff(newOrbit.AttitudeRatesRad, oldOrbit.AttitudeRatesRad) > thresholds.Orbit.Inclination.Value) return true;
        
        return false;
    }
}
