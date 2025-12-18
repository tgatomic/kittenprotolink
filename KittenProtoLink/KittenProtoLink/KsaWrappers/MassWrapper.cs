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
        if (Helpers.Diff(newMass.PropellantMass, oldMass.PropellantMass) > thresholds.Mass.PropellantMass) return true;
        if (Helpers.Diff(newMass.InertMass, oldMass.InertMass) > thresholds.Mass.InertMass) return true;
        if (Helpers.Diff(newMass.TotalMass, oldMass.TotalMass) > thresholds.Mass.TotalMass) return true;
        if (Helpers.Diff(newMass.DeltaVRemaining, oldMass.DeltaVRemaining) > thresholds.Mass.DeltaVRemaining) return true;
        if (Helpers.Diff(newMass.Twr, oldMass.Twr) > thresholds.Mass.ThrustWeightRatio) return true;

        return false;
    }
}