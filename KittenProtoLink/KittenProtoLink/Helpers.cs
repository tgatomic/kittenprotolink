using Brutal.Numerics;
using Ksa.Controller;

namespace KittenProtoLink;

public class Helpers
{
    public static double Diff(double a, double b) => Math.Abs(a - b);
    
    public static double Diff(Vector3d a, Vector3d b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        double dz = a.Z - b.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    
    public static double Diff(Vector3dInt a, Vector3dInt b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        double dz = a.Z - b.Z;
        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    
    public static double Diff(Quaterniond a, Quaterniond b)
    {
        // Dot product gives cosine of half the angle between them
        double dot = a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        // Clamp for safety (floating point)
        dot = Math.Clamp(dot, -1.0, 1.0);

        // Angle between orientations, radians
        double angle = 2.0 * Math.Acos(Math.Abs(dot));

        return angle; // radians
    }
    
    
    public static Vector3d ToVector3d(double3 value)
    {
        return new Vector3d { X = value.X, Y = value.Y, Z = value.Z };
    }
    
    public static Vector3d ToVector3d(int3 value)
    {
        return new Vector3d { X = value.X, Y = value.Y, Z = value.Z };
    }
    
    public static Vector3dInt ToVector3dInt(int3 value)
    {
        return new Vector3dInt
        {
            X = value.X,
            Y = value.Y,
            Z = value.Z
        };
    }

    public static Quaterniond ToQuaterniond(doubleQuat value)
    {
        return new Quaterniond { X = value.X, Y = value.Y, Z = value.Z, W = value.W };
    }
}