namespace KittenProtoLink;

public class TelemetryThresholds
{
    public required MassThresholds Mass { get; init; } 
    public required EngineThresholds Engine { get; init; } 
    public required OrbitThresholds Orbit { get; init; } 
    public required KinematicThresholds Kinematic { get; init; } 
    public required FlightComputerThresholds FlightComputer { get; init; } 
}

public class MassThresholds
{
    public double PropellantMass { get; init; } 
    public double InertMass { get; init; } 
    public double TotalMass { get; init; } 
    public double DeltaVRemaining { get; init; } 
    public double ThrustWeightRatio { get; init; } 
}
public class EngineThresholds
{
    public double FuelFlow { get; init; } 
    public double Throttle { get; init; } 
    public double Thrust { get; init; } 
}

public class OrbitThresholds
{
    public double Apoapsis { get; init; } 
    public double Periapsis { get; init; } 
    public double Inclination { get; init; } 
    public double Eccentricity { get; init; } 
    public double LongitudeOfAscendingNode { get; init; } 
    public double ArgumentOfPeriapsis { get; init; } 
    public double TrueAnomaly { get; init; } 
}

public class KinematicThresholds
{
    public double PositionEcl { get; init; } 
    public double VelocityEcl { get; init; } 
    public double AltitudeSurface { get; init; } 
    public double AltitudeRadar { get; init; } 
    public double SurfaceSpeed { get; init; } 
    public double InertialSpeed { get; init; } 
    public double BodyToEcl { get; init; } 
    public double AccelerationBody { get; init; } 
    public double AngularVelocity { get; init; } 
}

public class FlightComputerThresholds
{
    public double BurnTimeRemaining { get; init; } 
    public double BurnDvRemaining { get; init; } 
}