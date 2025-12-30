using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class OrbitWrapper (TelemetryThresholds thresholds)
{
    private OrbitTelemetry? _oldOrbit;
    
    public  OrbitTelemetry? BuildOrbitTelemetry(Vehicle vehicle)
    {
        var newOrbit = new OrbitTelemetry
        {
            Apoapsis = vehicle.Apoapsis,
            Periapsis = vehicle.Periapsis,
            Inclination = vehicle.Inclination,
            Eccentricity = vehicle.Eccentricity,
            LongitudeOfAscendingNode = vehicle.LongitudeOfAscendingNode,
            ArgumentOfPeriapsis = vehicle.ArgumentOfPeriapsis
        };
        
        if (!thresholds.Orbit.Apoapsis.Active)                  newOrbit.Apoapsis = 0;
        if (!thresholds.Orbit.Periapsis.Active)                 newOrbit.Periapsis = 0;
        if (!thresholds.Orbit.Inclination.Active)               newOrbit.Inclination = 0;
        if (!thresholds.Orbit.Eccentricity.Active)              newOrbit.Eccentricity = 0;
        if (!thresholds.Orbit.LongitudeOfAscendingNode.Active)  newOrbit.LongitudeOfAscendingNode = 0;
        if (!thresholds.Orbit.ArgumentOfPeriapsis.Active)       newOrbit.ArgumentOfPeriapsis = 0;

        if (_oldOrbit == null)
        {
            _oldOrbit = newOrbit;
            
            return newOrbit;
        }
        
        
        var result = HasSignificantChange(_oldOrbit, newOrbit)? newOrbit : null;
        _oldOrbit = newOrbit;

        return result;
    }
    
    private bool HasSignificantChange(OrbitTelemetry oldOrbit, OrbitTelemetry newOrbit)
    {
        if (thresholds.Orbit.Apoapsis.Active &&
            Helpers.Diff(newOrbit.Apoapsis, oldOrbit.Apoapsis) > thresholds.Orbit.Apoapsis.Value) return true;
        if (thresholds.Orbit.Periapsis.Active &&
            Helpers.Diff(newOrbit.Periapsis, oldOrbit.Periapsis) > thresholds.Orbit.Periapsis.Value) return true;
        if (thresholds.Orbit.Inclination.Active &&
            Helpers.Diff(newOrbit.Inclination, oldOrbit.Inclination) > thresholds.Orbit.Inclination.Value) return true;
        if (thresholds.Orbit.Eccentricity.Active &&
            Helpers.Diff(newOrbit.Eccentricity, oldOrbit.Eccentricity) > thresholds.Orbit.Eccentricity.Value) return true;
        if (thresholds.Orbit.LongitudeOfAscendingNode.Active &&
            Helpers.Diff(newOrbit.LongitudeOfAscendingNode, oldOrbit.LongitudeOfAscendingNode) > thresholds.Orbit.LongitudeOfAscendingNode.Value) return true;
        if (thresholds.Orbit.ArgumentOfPeriapsis.Active &&
            Helpers.Diff(newOrbit.ArgumentOfPeriapsis, oldOrbit.ArgumentOfPeriapsis) > thresholds.Orbit.ArgumentOfPeriapsis.Value) return true;
        
        return false;
    }
}
