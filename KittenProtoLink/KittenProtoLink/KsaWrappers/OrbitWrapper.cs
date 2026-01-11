using KSA;
using Ksa.Controller;

namespace KittenProtoLink.KsaWrappers;

public class OrbitWrapper (SettingsMenu settings)
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
        
        if (!settings.Thresholds.Orbit.Apoapsis.Active)                  newOrbit.Apoapsis = 0;
        if (!settings.Thresholds.Orbit.Periapsis.Active)                 newOrbit.Periapsis = 0;
        if (!settings.Thresholds.Orbit.Inclination.Active)               newOrbit.Inclination = 0;
        if (!settings.Thresholds.Orbit.Eccentricity.Active)              newOrbit.Eccentricity = 0;
        if (!settings.Thresholds.Orbit.LongitudeOfAscendingNode.Active)  newOrbit.LongitudeOfAscendingNode = 0;
        if (!settings.Thresholds.Orbit.ArgumentOfPeriapsis.Active)       newOrbit.ArgumentOfPeriapsis = 0;

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
        if (settings.Thresholds.Orbit.Apoapsis.Active &&
            Helpers.Diff(newOrbit.Apoapsis, oldOrbit.Apoapsis) > settings.Thresholds.Orbit.Apoapsis.Value) return true;
        if (settings.Thresholds.Orbit.Periapsis.Active &&
            Helpers.Diff(newOrbit.Periapsis, oldOrbit.Periapsis) > settings.Thresholds.Orbit.Periapsis.Value) return true;
        if (settings.Thresholds.Orbit.Inclination.Active &&
            Helpers.Diff(newOrbit.Inclination, oldOrbit.Inclination) > settings.Thresholds.Orbit.Inclination.Value) return true;
        if (settings.Thresholds.Orbit.Eccentricity.Active &&
            Helpers.Diff(newOrbit.Eccentricity, oldOrbit.Eccentricity) > settings.Thresholds.Orbit.Eccentricity.Value) return true;
        if (settings.Thresholds.Orbit.LongitudeOfAscendingNode.Active &&
            Helpers.Diff(newOrbit.LongitudeOfAscendingNode, oldOrbit.LongitudeOfAscendingNode) > settings.Thresholds.Orbit.LongitudeOfAscendingNode.Value) return true;
        if (settings.Thresholds.Orbit.ArgumentOfPeriapsis.Active &&
            Helpers.Diff(newOrbit.ArgumentOfPeriapsis, oldOrbit.ArgumentOfPeriapsis) > settings.Thresholds.Orbit.ArgumentOfPeriapsis.Value) return true;
        
        return false;
    }
}
