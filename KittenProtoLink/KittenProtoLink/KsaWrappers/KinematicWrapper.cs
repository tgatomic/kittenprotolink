using Brutal.Numerics;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class KinematicWrapper (SettingsMenu settings)
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
        
        if (!settings.Thresholds.Kinematic.PositionEcl.Active)      newKinematic.PositionEcl = default;
        if (!settings.Thresholds.Kinematic.VelocityEcl.Active)      newKinematic.VelocityEcl = default;
        if (!settings.Thresholds.Kinematic.AltitudeSurface.Active)  newKinematic.AltitudeSurface = 0;
        if (!settings.Thresholds.Kinematic.AltitudeRadar.Active)    newKinematic.AltitudeRadar = 0;
        if (!settings.Thresholds.Kinematic.SurfaceSpeed.Active)     newKinematic.SurfaceSpeed = 0;
        if (!settings.Thresholds.Kinematic.InertialSpeed.Active)    newKinematic.InertialSpeed = 0;
        if (!settings.Thresholds.Kinematic.BodyToEcl.Active)        newKinematic.BodyToEcl = null;
        if (!settings.Thresholds.Kinematic.AccelerationBody.Active) newKinematic.AccelerationBody = null;
        if (!settings.Thresholds.Kinematic.AngularVelocity.Active)  newKinematic.AngularVelocity = null;
        
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
        if (settings.Thresholds.Kinematic.PositionEcl.Active && oldMass.PositionEcl != null && newMass.PositionEcl != null &&
            Helpers.Diff(newMass.PositionEcl, oldMass.PositionEcl) > settings.Thresholds.Kinematic.PositionEcl.Value) return true;
        if (settings.Thresholds.Kinematic.VelocityEcl.Active && oldMass.VelocityEcl != null && newMass.VelocityEcl != null &&
            Helpers.Diff(newMass.VelocityEcl, oldMass.VelocityEcl) > settings.Thresholds.Kinematic.VelocityEcl.Value) return true;
        if (settings.Thresholds.Kinematic.AltitudeSurface.Active &&
            Helpers.Diff(newMass.AltitudeSurface, oldMass.AltitudeSurface) > settings.Thresholds.Kinematic.AltitudeSurface.Value) return true;
        if (settings.Thresholds.Kinematic.AltitudeRadar.Active &&
            Helpers.Diff(newMass.AltitudeRadar, oldMass.AltitudeRadar) > settings.Thresholds.Kinematic.AltitudeRadar.Value) return true;
        if (settings.Thresholds.Kinematic.SurfaceSpeed.Active &&
            Helpers.Diff(newMass.SurfaceSpeed, oldMass.SurfaceSpeed) > settings.Thresholds.Kinematic.SurfaceSpeed.Value) return true;
        if (settings.Thresholds.Kinematic.InertialSpeed.Active &&
            Helpers.Diff(newMass.InertialSpeed, oldMass.InertialSpeed) > settings.Thresholds.Kinematic.InertialSpeed.Value) return true;
        if (settings.Thresholds.Kinematic.BodyToEcl.Active && oldMass.BodyToEcl != null && newMass.BodyToEcl != null &&
            Helpers.Diff(newMass.BodyToEcl, oldMass.BodyToEcl) > settings.Thresholds.Kinematic.BodyToEcl.Value) return true;
        if (settings.Thresholds.Kinematic.AccelerationBody.Active && oldMass.AccelerationBody != null && newMass.AccelerationBody != null &&
            Helpers.Diff(newMass.AccelerationBody, oldMass.AccelerationBody) > settings.Thresholds.Kinematic.AccelerationBody.Value) return true;
        if (settings.Thresholds.Kinematic.AngularVelocity.Active && oldMass.AngularVelocity != null && newMass.AngularVelocity != null &&
            Helpers.Diff(newMass.AngularVelocity, oldMass.AngularVelocity) > settings.Thresholds.Kinematic.AngularVelocity.Value) return true;

        return false;
    }
    

}
