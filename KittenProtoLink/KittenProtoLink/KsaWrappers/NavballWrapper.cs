using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class NavballWrapper {
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
            
            // If NavBallData exposes these vectors, populate them:
            // (names here are guesses; check NavBallData in ILSpy)
            // telemetry.ProgradeDir    = Helpers.ToVector3d(nav.ProgradeDirection);
            // telemetry.RetrogradeDir  = Helpers.ToVector3d(nav.RetrogradeDirection);
            // telemetry.TargetDir      = Helpers.ToVector3d(nav.TargetDirection);
            // telemetry.ManeuverDir    = Helpers.ToVector3d(nav.ManeuverDirection);
            // ...etc...
        };

        if (_last == null || HasSignificantChange(_last, telemetry)) {
            _last = telemetry;
            return telemetry;
        }

        return null;
    }
    
    private static bool HasSignificantChange(NavballTelemetry oldOrbit, NavballTelemetry newOrbit)
    {
        if (oldOrbit.Frame != newOrbit.Frame) return true;
        if (Helpers.Diff(newOrbit.NavballToBody, oldOrbit.NavballToBody) > TelemetryThresholds.Apoapsis) return true;
        if (Helpers.Diff(newOrbit.AttitudeAnglesDeg, oldOrbit.AttitudeAnglesDeg) > TelemetryThresholds.Periapsis) return true;
        if (Helpers.Diff(newOrbit.AttitudeRatesRad, oldOrbit.AttitudeRatesRad) > TelemetryThresholds.Inclination) return true;
        
        return false;
    }
}