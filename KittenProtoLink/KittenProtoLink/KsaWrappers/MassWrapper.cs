using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class MassWrapper(SettingsMenu settings)
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
        if (!settings.Thresholds.Mass.PropellantMass.Active)  newMass.PropellantMass = 0;
        if (!settings.Thresholds.Mass.InertMass.Active)       newMass.InertMass = 0;
        if (!settings.Thresholds.Mass.TotalMass.Active)       newMass.TotalMass = 0;
        if (!settings.Thresholds.Mass.DeltaVRemaining.Active) newMass.DeltaVRemaining = 0;
        if (!settings.Thresholds.Mass.ThrustWeightRatio.Active) newMass.Twr = 0;
        
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
        if (settings.Thresholds.Mass.PropellantMass.Active &&
            Helpers.Diff(newMass.PropellantMass, oldMass.PropellantMass) > settings.Thresholds.Mass.PropellantMass.Value) return true;
        if (settings.Thresholds.Mass.InertMass.Active &&
            Helpers.Diff(newMass.InertMass, oldMass.InertMass) > settings.Thresholds.Mass.InertMass.Value) return true;
        if (settings.Thresholds.Mass.TotalMass.Active &&
            Helpers.Diff(newMass.TotalMass, oldMass.TotalMass) > settings.Thresholds.Mass.TotalMass.Value) return true;
        if (settings.Thresholds.Mass.DeltaVRemaining.Active &&
            Helpers.Diff(newMass.DeltaVRemaining, oldMass.DeltaVRemaining) > settings.Thresholds.Mass.DeltaVRemaining.Value) return true;
        if (settings.Thresholds.Mass.ThrustWeightRatio.Active &&
            Helpers.Diff(newMass.Twr, oldMass.Twr) > settings.Thresholds.Mass.ThrustWeightRatio.Value) return true;

        return false;
    }
}
