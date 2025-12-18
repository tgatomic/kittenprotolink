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
        if (Helpers.Diff(newOrbit.Apoapsis, oldOrbit.Apoapsis) > thresholds.Orbit.Apoapsis) return true;
        if (Helpers.Diff(newOrbit.Periapsis, oldOrbit.Periapsis) > thresholds.Orbit.Periapsis) return true;
        if (Helpers.Diff(newOrbit.Inclination, oldOrbit.Inclination) > thresholds.Orbit.Inclination) return true;
        if (Helpers.Diff(newOrbit.Eccentricity, oldOrbit.Eccentricity) > thresholds.Orbit.Eccentricity) return true;
        if (Helpers.Diff(newOrbit.LongitudeOfAscendingNode, oldOrbit.LongitudeOfAscendingNode) > thresholds.Orbit.LongitudeOfAscendingNode) return true;
        if (Helpers.Diff(newOrbit.ArgumentOfPeriapsis, oldOrbit.ArgumentOfPeriapsis) > thresholds.Orbit.ArgumentOfPeriapsis) return true;
        
        return false;
    }
}