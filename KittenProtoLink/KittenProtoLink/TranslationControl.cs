using System.Reflection;
using KittenProtoLink.KsaWrappers;
using KSA;
using Ksa.Controller;

namespace KittenProtoLink;

public class TranslationControl
{
    static readonly FieldInfo ManualInputsField =
        typeof(KSA.Vehicle).GetField("_manualControlInputs",
            BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("field not found");
    
    
    public void HandleCommand(ManualControlCommand cmd)
    {
        var vehicle = Program.ControlledVehicle;
        if (vehicle == null)
            return;
        
        if (cmd.Command != null)
        {
            var requestedEngineState = cmd.Command.EngineOn;
            var current = EngineWrapper.IsEngineEnabled(vehicle);
            if (requestedEngineState != current)
            {
                vehicle.SetEnum(requestedEngineState ? VehicleEngine.MainIgnite : VehicleEngine.MainShutdown);
            }

            var currentThrottle = vehicle.GetManualThrottle();
            if (Helpers.Diff(currentThrottle, cmd.Command.Throttle) > TelemetryThresholds.Throttle)
            {
                SetThrottle(vehicle, cmd.Command.Throttle);
            }

            if (cmd.Command.Rotation != null)
            {
                var roll = cmd.Command.Rotation;
                SetRotation(vehicle, roll.PitchUp, roll.PitchDown, roll.YawLeft, roll.YawRight,
                    roll.RollLeft, roll.RollRight);
            }

            if (cmd.Command.Translate != null)
            {
                var trans = cmd.Command.Translate;
                SetTranslation(vehicle, trans.TranslateForward, trans.TranslateBackward,
                    trans.TranslateLeft, trans.TranslateRight, trans.TranslateUp, trans.TranslateDown);
            } 
        }

        foreach (var action in cmd.Actions)
        {
            switch (action)
            {
                case ControlAction.ToggleManualThrust:
                    EngineWrapper.ToggleEngine(vehicle);
                    break;
                case ControlAction.ToggleStabilization:
                    vehicle.ToggleStabilization();
                    break;
                case ControlAction.ResetNavballFrame:
                    vehicle.SetNavBallFrame(vehicle.VehicleRegion.GetVehicleReferenceFrame());
                    break;
            }
        }
    }
    
    private void SetThrottle(KSA.Vehicle vehicle, int throttlePercent)
    {
        var throttleFloat = Math.Clamp(((float)throttlePercent/100), vehicle.GetMinThrottle(), 1f);

        var manualInputs = (ManualControlInputs)ManualInputsField.GetValue(vehicle)!;
        manualInputs.EngineThrottle = throttleFloat;
        ManualInputsField.SetValue(vehicle, manualInputs);
    }
    
    private void SetThrottle(KSA.Vehicle vehicle, float throttleFloat)
    {
        var manualInputs = (ManualControlInputs)ManualInputsField.GetValue(vehicle)!;
        manualInputs.EngineThrottle = throttleFloat;
        
        Console.WriteLine("Setting throttle to " + manualInputs);
        ManualInputsField.SetValue(vehicle, manualInputs);
    }
    
    private void SetThrusterFlags(KSA.Vehicle vehicle, ThrusterMapFlags flags)
    {
        var inputs = (ManualControlInputs)ManualInputsField.GetValue(vehicle)!;
        inputs.ThrusterCommandFlags = flags;
        ManualInputsField.SetValue(vehicle, inputs);
    }
    
    private void SetTranslation(KSA.Vehicle vehicle, bool forward, bool back,
                        bool left, bool right, bool up, bool down)
    {
        var flags = ThrusterMapFlags.None;
        if (forward) flags |= ThrusterMapFlags.TranslateForward;
        if (back)    flags |= ThrusterMapFlags.TranslateBackward;
        if (left)    flags |= ThrusterMapFlags.TranslateLeft;
        if (right)   flags |= ThrusterMapFlags.TranslateRight;
        if (up)      flags |= ThrusterMapFlags.TranslateUp;
        if (down)    flags |= ThrusterMapFlags.TranslateDown;

        var inputs = (ManualControlInputs)ManualInputsField.GetValue(vehicle)!;
        inputs.ThrusterCommandFlags &= ~(ThrusterMapFlags.TranslateForward |
                                         ThrusterMapFlags.TranslateBackward |
                                         ThrusterMapFlags.TranslateLeft |
                                         ThrusterMapFlags.TranslateRight |
                                         ThrusterMapFlags.TranslateUp |
                                         ThrusterMapFlags.TranslateDown);
        inputs.ThrusterCommandFlags |= flags;
        ManualInputsField.SetValue(vehicle, inputs);
    }

    private void SetRotation(KSA.Vehicle vehicle, bool pitchUp, bool pitchDown,
                     bool yawLeft, bool yawRight, bool rollLeft, bool rollRight)
    {
        var flags = ThrusterMapFlags.None;
        if (pitchUp)   flags |= ThrusterMapFlags.PitchUp;
        if (pitchDown) flags |= ThrusterMapFlags.PitchDown;
        if (yawLeft)   flags |= ThrusterMapFlags.YawLeft;
        if (yawRight)  flags |= ThrusterMapFlags.YawRight;
        if (rollLeft)  flags |= ThrusterMapFlags.RollLeft;
        if (rollRight) flags |= ThrusterMapFlags.RollRight;

        var inputs = (ManualControlInputs)ManualInputsField.GetValue(vehicle);
        inputs.ThrusterCommandFlags &= ~(ThrusterMapFlags.PitchUp | ThrusterMapFlags.PitchDown |
                                         ThrusterMapFlags.YawLeft  | ThrusterMapFlags.YawRight  |
                                         ThrusterMapFlags.RollLeft | ThrusterMapFlags.RollRight);
        inputs.ThrusterCommandFlags |= flags;
        ManualInputsField.SetValue(vehicle, inputs);
    }
}