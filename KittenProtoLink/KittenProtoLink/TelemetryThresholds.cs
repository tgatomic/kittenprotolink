using KittenProtoLink.Data;

namespace KittenProtoLink;

public class TelemetryThresholds
{
    public MassThresholds Mass { get; set; } = new(); 
    public EngineThresholds Engine { get; set; } = new(); 
    public OrbitThresholds Orbit { get; set; } = new(); 
    public KinematicThresholds Kinematic { get; set; } = new(); 
    public FlightComputerThresholds FlightComputer { get; set; } = new(); 
}

public class MassThresholds
{
    public ThresholdInformation PropellantMass { get; set; } = new(); 
    public ThresholdInformation InertMass { get; set; } = new(); 
    public ThresholdInformation TotalMass { get; set; } = new(); 
    public ThresholdInformation DeltaVRemaining { get; set; } = new(); 
    public ThresholdInformation ThrustWeightRatio { get; set; } = new(); 
}
public class EngineThresholds
{
    public ThresholdInformation FuelFlow { get; set; } = new(); 
    public ThresholdInformation Throttle { get; set; } = new(); 
    public ThresholdInformation Thrust { get; set; } = new(); 
}

public class OrbitThresholds
{
    public ThresholdInformation Apoapsis { get; set; } = new(); 
    public ThresholdInformation Periapsis { get; set; } = new(); 
    public ThresholdInformation Inclination { get; set; } = new(); 
    public ThresholdInformation Eccentricity { get; set; } = new(); 
    public ThresholdInformation LongitudeOfAscendingNode { get; set; } = new(); 
    public ThresholdInformation ArgumentOfPeriapsis { get; set; } = new(); 
}

public class KinematicThresholds
{
    public ThresholdInformation PositionEcl { get; set; } = new(); 
    public ThresholdInformation VelocityEcl { get; set; } = new(); 
    public ThresholdInformation AltitudeSurface { get; set; } = new(); 
    public ThresholdInformation AltitudeRadar { get; set; } = new(); 
    public ThresholdInformation SurfaceSpeed { get; set; } = new(); 
    public ThresholdInformation InertialSpeed { get; set; } = new(); 
    public ThresholdInformation BodyToEcl { get; set; } = new(); 
    public ThresholdInformation AccelerationBody { get; set; } = new(); 
    public ThresholdInformation AngularVelocity { get; set; } = new(); 
}

public class FlightComputerThresholds
{
    public ThresholdInformation BurnTimeRemaining { get; set; } = new(); 
    public ThresholdInformation BurnDvRemaining { get; set; } = new(); 
}
