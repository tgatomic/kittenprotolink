namespace KittenProtoLink;

public class TelemetryThresholds
{
    public MassThresholds Mass { get; set; } 
    public EngineThresholds Engine { get; set; } 
    public OrbitThresholds Orbit { get; set; } 
    public KinematicThresholds Kinematic { get; set; } 
    public  FlightComputerThresholds FlightComputer { get; set; } 
}

public class MassThresholds
{
    public double PropellantMass { get; set; } 
    public double InertMass { get; set; } 
    public double TotalMass { get; set; } 
    public double DeltaVRemaining { get; set; } 
    public double ThrustWeightRatio { get; set; } 
}
public class EngineThresholds
{
    public double FuelFlow { get; set; } 
    public double Throttle { get; set; } 
    public double Thrust { get; set; } 
}

public class OrbitThresholds
{
    public double Apoapsis { get; set; } 
    public double Periapsis { get; set; } 
    public double Inclination { get; set; } 
    public double Eccentricity { get; set; } 
    public double LongitudeOfAscendingNode { get; set; } 
    public double ArgumentOfPeriapsis { get; set; } 
    public double TrueAnomaly { get; set; } 
}

public class KinematicThresholds
{
    public double PositionEcl { get; set; } 
    public double VelocityEcl { get; set; } 
    public double AltitudeSurface { get; set; } 
    public double AltitudeRadar { get; set; } 
    public double SurfaceSpeed { get; set; } 
    public double InertialSpeed { get; set; } 
    public double BodyToEcl { get; set; } 
    public double AccelerationBody { get; set; } 
    public double AngularVelocity { get; set; } 
}

public class FlightComputerThresholds
{
    public double BurnTimeRemaining { get; set; } 
    public double BurnDvRemaining { get; set; } 
}