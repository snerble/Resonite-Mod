using System.Numerics;

namespace Snerble.Utilities;

public static class VectorUtility
{
    public static float Dot(this Vector2 a, Vector2 b) => Vector2.Dot(a, b);
    public static float Acos(this Vector2 vector) => MathF.Acos(vector.Dot(Vector2.UnitY));
    public static Vector2 Flip(this Vector2 vector) => new(vector.Y, vector.X);
    public static Vector2 Abs(this Vector2 vector) => Vector2.Abs(vector);
    public static Vector2 Normalize(this Vector2 vector) => Vector2.Normalize(vector);
    public static Vector2 CoalesceNaN(this Vector2 vector) => new(float.IsNaN(vector.X) ? 0 : vector.X, float.IsNaN(vector.Y) ? 0 : vector.Y);
    public static Vector2 Round(this Vector2 vector, int decimals = 0) => new(MathF.Round(vector.X, decimals), MathF.Round(vector.Y, decimals));

    public static Vector2 FromAngle(float angle)
    {
        var (x, y) = MathF.SinCos(angle);
        return new(x, y);
    }
}
