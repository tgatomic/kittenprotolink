using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class MassWrapper(TelemetryThresholds thresholds)
{
    private MassTelemetry? _oldMass;

    public MassTelemetry? BuildMassTelemetry(Vehicle vehicle)
    {
        var nav = vehicle.NavBallData;
        
        var newMass = new MassTelemetry
        {
            PropellantMass = vehicle.PropellantMass,
            InertMass = vehicle.InertMass,
            TotalMass = vehicle.TotalMass,
            DeltaVRemaining = nav.DeltaVInVacuum,
            Twr = nav.ThrustWeightRatio
        };
        
        // Mask inactive => default(0) so proto omits them
        if (!thresholds.Mass.PropellantMass.Active)  newMass.PropellantMass = 0;
        if (!thresholds.Mass.InertMass.Active)       newMass.InertMass = 0;
        if (!thresholds.Mass.TotalMass.Active)       newMass.TotalMass = 0;
        if (!thresholds.Mass.DeltaVRemaining.Active) newMass.DeltaVRemaining = 0;
        if (!thresholds.Mass.ThrustWeightRatio.Active) newMass.Twr = 0;
        
        if (_oldMass == null)
        {
            _oldMass = newMass;

            return newMass;
        }
        
        var result = HasSignificantChange(_oldMass, newMass)? newMass: null;
        
        _oldMass = newMass;
        
        return result;
    }
    
    private bool HasSignificantChange(MassTelemetry oldMass, MassTelemetry newMass)
    {
        if (thresholds.Mass.PropellantMass.Active &&
            Helpers.Diff(newMass.PropellantMass, oldMass.PropellantMass) > thresholds.Mass.PropellantMass.Value) return true;
        if (thresholds.Mass.InertMass.Active &&
            Helpers.Diff(newMass.InertMass, oldMass.InertMass) > thresholds.Mass.InertMass.Value) return true;
        if (thresholds.Mass.TotalMass.Active &&
            Helpers.Diff(newMass.TotalMass, oldMass.TotalMass) > thresholds.Mass.TotalMass.Value) return true;
        if (thresholds.Mass.DeltaVRemaining.Active &&
            Helpers.Diff(newMass.DeltaVRemaining, oldMass.DeltaVRemaining) > thresholds.Mass.DeltaVRemaining.Value) return true;
        if (thresholds.Mass.ThrustWeightRatio.Active &&
            Helpers.Diff(newMass.Twr, oldMass.Twr) > thresholds.Mass.ThrustWeightRatio.Value) return true;

        return false;
    }
}
