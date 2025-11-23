using Brutal.Numerics;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class KinematicWrapper
{
    private KinematicTelemetry? _oldKinematicTelemetry;
    
    public KinematicTelemetry? BuildKinematicsTelemetry(Vehicle vehicle)
    {
        var measurements = vehicle.KinematicMeasurements;
        
        var newKinematic = new KinematicTelemetry
        {
            PositionEcl = Helpers.ToVector3d(vehicle.GetPositionEcl()),
            VelocityEcl = Helpers.ToVector3d(vehicle.GetVelocityEcl()),
            AltitudeSurface = vehicle.GetBarometricAltitude(),
            AltitudeRadar = vehicle.GetRadarAltitude(),
            SurfaceSpeed = vehicle.GetSurfaceSpeed(),
            InertialSpeed = vehicle.GetInertialSpeed(),
            BodyToEcl = Helpers.ToQuaterniond(vehicle.Body2Cce),
            AccelerationBody = Helpers.ToVector3d(measurements.AccelerationBody),
            AngularVelocity = Helpers.ToVector3d(vehicle.BodyRates)
        };

        if (_oldKinematicTelemetry == null)
        {
            _oldKinematicTelemetry = newKinematic;

            return newKinematic;
        }
        
        var result = HasSignificantChange(_oldKinematicTelemetry, newKinematic)? _oldKinematicTelemetry: null;
        
        _oldKinematicTelemetry = newKinematic;
        
        return result;
    }
    
    private static bool HasSignificantChange(KinematicTelemetry oldMass, KinematicTelemetry newMass)
    {
        if (Helpers.Diff(newMass.PositionEcl, oldMass.PositionEcl) > TelemetryThresholds.PositionEcl) return true;
        if (Helpers.Diff(newMass.VelocityEcl, oldMass.VelocityEcl) > TelemetryThresholds.VelocityEcl) return true;
        if (Helpers.Diff(newMass.AltitudeSurface, oldMass.AltitudeSurface) > TelemetryThresholds.AltitudeSurface) return true;
        if (Helpers.Diff(newMass.AltitudeRadar, oldMass.AltitudeRadar) > TelemetryThresholds.AltitudeRadar) return true;
        if (Helpers.Diff(newMass.SurfaceSpeed, oldMass.SurfaceSpeed) > TelemetryThresholds.SurfaceSpeed) return true;
        if (Helpers.Diff(newMass.InertialSpeed, oldMass.InertialSpeed) > TelemetryThresholds.InertialSpeed) return true;
        if (Helpers.Diff(newMass.BodyToEcl, oldMass.BodyToEcl) > TelemetryThresholds.BodyToEcl) return true;
        if (Helpers.Diff(newMass.AccelerationBody, oldMass.AccelerationBody) > TelemetryThresholds.AccelerationBody) return true;
        if (Helpers.Diff(newMass.AngularVelocity, oldMass.AngularVelocity) > TelemetryThresholds.AngularVelocity) return true;

        return false;
    }
    

}