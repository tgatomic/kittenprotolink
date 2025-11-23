namespace KittenProtoLink.KsaWrappers;

public static class TelemetryThresholds
{
    // Mass thresholds
    public static double PropellantMass => 0.01;
    public static double InertMass => 0.01;
    public static double TotalMass => 0.01;
    public static double DeltaVRemaining =>  0.01;
    public static double ThrustWeightRatio => 0.01;
    
    // Engine thresholds
    public static double FuelFlow => 10.0;
    public static double Throttle => 0.001;
    public static double Thrust => 0.01;
    
    // Vehicle thresholds

    
    // Orbit thresholds
    public static double Apoapsis => 0.01;
    public static double Periapsis => 0.01;
    public static double Inclination => 0.01;
    public static double Eccentricity => 0.01;
    public static double LongitudeOfAscendingNode => 0.01;
    public static double ArgumentOfPeriapsis => 0.01;
    public static double TrueAnomaly => 0.01;

    // Kinematic thresholds
    public static double PositionEcl => 0.01;
    public static double VelocityEcl => 0.01;
    public static double AltitudeSurface => 0.01;
    public static double AltitudeRadar => 0.01;
    public static double SurfaceSpeed => 0.01;
    public static double InertialSpeed => 0.01;
    public static double BodyToEcl => 0.01;
    public static double AccelerationBody => 0.01;
    public static double AngularVelocity => 0.01;
    
    // Flight Computer thresholds
    public static double BurnTimeRemaining => 0.01;
    public static double BurnDvRemaining => 0.01;





}
