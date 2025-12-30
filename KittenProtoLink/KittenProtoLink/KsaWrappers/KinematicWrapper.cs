using Brutal.Numerics;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class KinematicWrapper (TelemetryThresholds thresholds)
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
        
        if (!thresholds.Kinematic.PositionEcl.Active)      newKinematic.PositionEcl = default;
        if (!thresholds.Kinematic.VelocityEcl.Active)      newKinematic.VelocityEcl = default;
        if (!thresholds.Kinematic.AltitudeSurface.Active)  newKinematic.AltitudeSurface = 0;
        if (!thresholds.Kinematic.AltitudeRadar.Active)    newKinematic.AltitudeRadar = 0;
        if (!thresholds.Kinematic.SurfaceSpeed.Active)     newKinematic.SurfaceSpeed = 0;
        if (!thresholds.Kinematic.InertialSpeed.Active)    newKinematic.InertialSpeed = 0;
        if (!thresholds.Kinematic.BodyToEcl.Active)        newKinematic.BodyToEcl = null;
        if (!thresholds.Kinematic.AccelerationBody.Active) newKinematic.AccelerationBody = null;
        if (!thresholds.Kinematic.AngularVelocity.Active)  newKinematic.AngularVelocity = null;
        
        if (_oldKinematicTelemetry == null)
        {
            _oldKinematicTelemetry = newKinematic;

            return newKinematic;
        }
        
        var changed = HasSignificantChange(_oldKinematicTelemetry, newKinematic);
        _oldKinematicTelemetry = newKinematic;
        
        return changed ? newKinematic : null;
    }
    
    private bool HasSignificantChange(KinematicTelemetry oldMass, KinematicTelemetry newMass)
    {
        if (thresholds.Kinematic.PositionEcl.Active &&
            Helpers.Diff(newMass.PositionEcl, oldMass.PositionEcl) > thresholds.Kinematic.PositionEcl.Value) return true;
        if (thresholds.Kinematic.VelocityEcl.Active &&
            Helpers.Diff(newMass.VelocityEcl, oldMass.VelocityEcl) > thresholds.Kinematic.VelocityEcl.Value) return true;
        if (thresholds.Kinematic.AltitudeSurface.Active &&
            Helpers.Diff(newMass.AltitudeSurface, oldMass.AltitudeSurface) > thresholds.Kinematic.AltitudeSurface.Value) return true;
        if (thresholds.Kinematic.AltitudeRadar.Active &&
            Helpers.Diff(newMass.AltitudeRadar, oldMass.AltitudeRadar) > thresholds.Kinematic.AltitudeRadar.Value) return true;
        if (thresholds.Kinematic.SurfaceSpeed.Active &&
            Helpers.Diff(newMass.SurfaceSpeed, oldMass.SurfaceSpeed) > thresholds.Kinematic.SurfaceSpeed.Value) return true;
        if (thresholds.Kinematic.InertialSpeed.Active &&
            Helpers.Diff(newMass.InertialSpeed, oldMass.InertialSpeed) > thresholds.Kinematic.InertialSpeed.Value) return true;
        if (thresholds.Kinematic.BodyToEcl.Active &&
            Helpers.Diff(newMass.BodyToEcl, oldMass.BodyToEcl) > thresholds.Kinematic.BodyToEcl.Value) return true;
        if (thresholds.Kinematic.AccelerationBody.Active &&
            Helpers.Diff(newMass.AccelerationBody, oldMass.AccelerationBody) > thresholds.Kinematic.AccelerationBody.Value) return true;
        if (thresholds.Kinematic.AngularVelocity.Active &&
            Helpers.Diff(newMass.AngularVelocity, oldMass.AngularVelocity) > thresholds.Kinematic.AngularVelocity.Value) return true;

        return false;
    }
    

}
