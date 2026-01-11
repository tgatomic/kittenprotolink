using System.Reflection;
using System.Text.Json;
using Brutal.ImGuiApi;
using KittenProtoLink.Data;
using ModMenu;

namespace KittenProtoLink;

public class SettingsMenu
{
    private TelemetryThresholds? _thresholds;
   
    public TelemetryThresholds Thresholds { 
        get => _thresholds ?? throw new InvalidOperationException();
        set => _thresholds = value;
    }
    
    private const string ThresholdsJsonFile = "KittenProtoLinkSettings.json";
    private const string ThresholdsTemplateJsonFile = "Thresholds_template.json";
    private static CancellationTokenSource? _saveCts;

    private string fullFilePath;

    public SettingsMenu()
    {
        var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var myGamesDir = Path.Combine(docs, "My Games");
        var targetDir = Path.Combine(myGamesDir, "Kitten Space Agency");
        fullFilePath = Path.Combine(targetDir, ThresholdsJsonFile);
        
        if (File.Exists(fullFilePath))
        {
            var thresholdsRaw = File.ReadAllText(fullFilePath);
            try
            {
                _thresholds = JsonSerializer.Deserialize<TelemetryThresholds>(thresholdsRaw)!;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to decode Thresholds.json: " + e.Message);
            }
        }
        else
        {
            var dllPath = Assembly.GetExecutingAssembly().Location;
            var dllDir  = Path.GetDirectoryName(dllPath)!;
            
            if (!File.Exists(Path.Combine(dllDir, ThresholdsTemplateJsonFile)))
            {
                throw new FileNotFoundException("Thresholds template file not found");
            }
            
            File.WriteAllText(fullFilePath, File.ReadAllText(Path.Combine(dllDir, ThresholdsTemplateJsonFile)));
            
            var thresholdsRaw = File.ReadAllText(fullFilePath);
            try
            {
                _thresholds = JsonSerializer.Deserialize<TelemetryThresholds>(thresholdsRaw)!;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to decode Thresholds.json: " + e.Message);
            }
        }
    }
    
    private void RequestSave()
    {
        _saveCts?.Cancel();
        _saveCts = new CancellationTokenSource();
        var token = _saveCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                var json = JsonSerializer.Serialize(_thresholds, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(fullFilePath, json);
            }
            catch (TaskCanceledException) { /* debounced */ }
        }, token);
    }
    
    [ModMenuEntry("Kitten Proto-Link")]
    public void DrawSubMenuEntry()
    {
        if (_thresholds == null)
        {
            ImGui.Text("Thresholds not loaded.");
            return;
        }

        if (!ImGui.BeginMenu("Thresholds"))
            return;

        ImGui.Text("Set telemetry change thresholds");
        ImGui.Separator();
        
        if (ImGui.CollapsingHeader("Orbit"))
        {
            DrawThreshold("Apoapsis", _thresholds.Orbit.Apoapsis, 10f, 0f, 2_000_000f);
            DrawThreshold("Periapsis", _thresholds.Orbit.Periapsis, 10f, 0f, 2_000_000f);
            DrawThreshold("Inclination", _thresholds.Orbit.Inclination, 0.01f, 0f, 180f);
            DrawThreshold("Eccentricity", _thresholds.Orbit.Eccentricity, 0.001f, 0f, 2f);
            DrawThreshold("LAN", _thresholds.Orbit.LongitudeOfAscendingNode, 0.01f, -360f, 360f);
            DrawThreshold("Argument of Periapsis", _thresholds.Orbit.ArgumentOfPeriapsis, 0.01f, -360f, 360f);
        }

        if (ImGui.CollapsingHeader("Navball"))
        {
            DrawThreshold("Navball->Body", _thresholds.Orbit.Apoapsis, 0.001f, 0f, 10f);
            DrawThreshold("Attitude Angles", _thresholds.Orbit.Periapsis, 0.001f, 0f, 10f);
            DrawThreshold("Attitude Rates", _thresholds.Orbit.Inclination, 0.001f, 0f, 10f);
        }

        if (ImGui.CollapsingHeader("Vehicle Mass"))
        {
            DrawThreshold("Propellant Mass", _thresholds.Mass.PropellantMass, 1f, 0f, 1_000_000f);
            DrawThreshold("Inert Mass", _thresholds.Mass.InertMass, 1f, 0f, 1_000_000f);
            DrawThreshold("Total Mass", _thresholds.Mass.TotalMass, 1f, 0f, 1_000_000f);
            DrawThreshold("Delta-V Remaining", _thresholds.Mass.DeltaVRemaining, 1f, 0f, 100_000f);
            DrawThreshold("TWR", _thresholds.Mass.ThrustWeightRatio, 0.01f, 0f, 50f);
        }

        if (ImGui.CollapsingHeader("Engine"))
        {
            DrawThreshold("Fuel Flow", _thresholds.Engine.FuelFlow, 0.1f, 0f, 10_000f);
            DrawThreshold("Throttle", _thresholds.Engine.Throttle, 0.01f, 0f, 1f);
            DrawThreshold("Thrust", _thresholds.Engine.Thrust, 0.1f, 0f, 1_000_000f);
        }

        if (ImGui.CollapsingHeader("Kinematic"))
        {
            DrawThreshold("Position Ecl", _thresholds.Kinematic.PositionEcl, 10f, 0f, 1_000_000f);
            DrawThreshold("Velocity Ecl", _thresholds.Kinematic.VelocityEcl, 1f, 0f, 100_000f);
            DrawThreshold("Altitude Surface", _thresholds.Kinematic.AltitudeSurface, 1f, 0f, 1_000_000f);
            DrawThreshold("Altitude Radar", _thresholds.Kinematic.AltitudeRadar, 1f, 0f, 1_000_000f);
            DrawThreshold("Surface Speed", _thresholds.Kinematic.SurfaceSpeed, 0.1f, 0f, 100_000f);
            DrawThreshold("Inertial Speed", _thresholds.Kinematic.InertialSpeed, 0.1f, 0f, 100_000f);
            DrawThreshold("Body->Ecl", _thresholds.Kinematic.BodyToEcl, 0.001f, 0f, 10f);
            DrawThreshold("Acceleration Body", _thresholds.Kinematic.AccelerationBody, 0.1f, 0f, 100_000f);
            DrawThreshold("Angular Velocity", _thresholds.Kinematic.AngularVelocity, 0.001f, 0f, 10f);
        }

        if (ImGui.CollapsingHeader("Flight Computer"))
        {
            DrawThreshold("Burn Time Remaining", _thresholds.FlightComputer.BurnTimeRemaining, 0.1f, 0f, 10_000f);
            DrawThreshold("Burn DV Remaining", _thresholds.FlightComputer.BurnDvRemaining, 0.1f, 0f, 100_000f);
        }
        
        ImGui.EndMenu();
    }

    private void DrawThreshold(string label, ThresholdInformation info, float speed, float min, float max)
    {
        var active = info.Active;
        if (ImGui.Checkbox($"Send {label}", ref active))
            info.Active = active;

        ImGui.SameLine();

        var value = (float)info.Value;
        if (ImGui.DragFloat($"{label} Delta", ref value, speed, min, max))
        {
            info.Value = value;
            RequestSave();
        }

        ImGui.SameLine();
        if (ImGui.SmallButton($"Reset##{label}"))
        { 
            info.Value = info.DefaultValue;
            info.Active = true;
            RequestSave();
        }
    }
}
