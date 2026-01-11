using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class NavballWrapper (SettingsMenu settings)
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
        
        if (!settings.Thresholds.Orbit.Apoapsis.Active)     telemetry.NavballToBody = null;
        if (!settings.Thresholds.Orbit.Periapsis.Active)    telemetry.AttitudeAnglesDeg = null;
        if (!settings.Thresholds.Orbit.Inclination.Active)  telemetry.AttitudeRatesRad = null;

        if (_last == null || HasSignificantChange(_last, telemetry)) {
            _last = telemetry;
            return telemetry;
        }

        return null;
    }
    
    private bool HasSignificantChange(NavballTelemetry oldOrbit, NavballTelemetry newOrbit)
    {
        if (oldOrbit.Frame != newOrbit.Frame) return true;
        if (settings.Thresholds.Orbit.Apoapsis.Active && newOrbit.NavballToBody != null && oldOrbit.NavballToBody != null &&
            Helpers.Diff(newOrbit.NavballToBody, oldOrbit.NavballToBody) > settings.Thresholds.Orbit.Apoapsis.Value) return true;
        if (settings.Thresholds.Orbit.Periapsis.Active && newOrbit.AttitudeAnglesDeg != null && oldOrbit.AttitudeAnglesDeg != null &&
            Helpers.Diff(newOrbit.AttitudeAnglesDeg, oldOrbit.AttitudeAnglesDeg) > settings.Thresholds.Orbit.Periapsis.Value) return true;
        if (settings.Thresholds.Orbit.Inclination.Active && newOrbit.AttitudeRatesRad != null && oldOrbit.AttitudeRatesRad != null &&
            Helpers.Diff(newOrbit.AttitudeRatesRad, oldOrbit.AttitudeRatesRad) > settings.Thresholds.Orbit.Inclination.Value) return true;
        
        return false;
    }
}
