using KittenProtoLink.KsaWrappers;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink;

public class TelemetryBuilder(TelemetryThresholds thresholds)
{
    private const uint ProtocolVersion = 1;
    private uint _sequence;

    private readonly VehicleWrapper _vehicleWrapper = new(thresholds);
    private readonly OrbitWrapper _orbitWrapper = new(thresholds);
    private readonly KinematicWrapper _kinematicWrapper = new(thresholds);
    private readonly FlightControllerWrapper _flightWrapper = new(thresholds);
    private readonly NavballWrapper _navballWrapper = new(thresholds);

    public Envelope? BuildTelemetryEnvelope(Vehicle vehicle)
    {
        var telemetry = new Telemetry
        {
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VesselId = vehicle.Id,
        };

        var flightWrapper = _flightWrapper.BuildFlightComputerTelemetry(vehicle);
        if (flightWrapper != null)
            telemetry.FlightComp = flightWrapper;
        
        var kinematicWrapper = _kinematicWrapper.BuildKinematicsTelemetry(vehicle);
        if  (kinematicWrapper != null)
            telemetry.Kinematics = kinematicWrapper;
        
        var orbitTelemetry = _orbitWrapper.BuildOrbitTelemetry(vehicle);
        if (orbitTelemetry != null)
            telemetry.Orbit = orbitTelemetry;
        
        var vehicleTelemetry = _vehicleWrapper.BuildVehicleTelemetry(vehicle);
        if (vehicleTelemetry != null)
            telemetry.Vehicle = vehicleTelemetry;

        var navball = _navballWrapper.BuildNavballTelemetry(vehicle);
        if  (navball != null)
            telemetry.Navball = navball;

        if (flightWrapper != null || kinematicWrapper != null ||
            orbitTelemetry != null || vehicleTelemetry != null || navball != null)
        {
            return new Envelope
            {
                ProtocolVersion = ProtocolVersion,
                Sequence = ++_sequence,
                Telemetry = telemetry
            };
        }

        return null;
    }
}