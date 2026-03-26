using System;

namespace GameCore
{
    /// <summary>
    /// Simple 3D vector struct.
    /// </summary>
    public struct Vector3 : IEquatable<Vector3>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Zero => new(0, 0, 0);
        public static Vector3 One => new(1, 1, 1);
        public static Vector3 Up => new(0, 1, 0);
        public static Vector3 Down => new(0, -1, 0);
        public static Vector3 Forward => new(0, 0, 1);
        public static Vector3 Back => new(0, 0, -1);
        public static Vector3 Right => new(1, 0, 0);
        public static Vector3 Left => new(-1, 0, 0);

        public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);
        public Vector3 Normalized => this / Magnitude;

        public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 v, float scalar) => new(v.X * scalar, v.Y * scalar, v.Z * scalar);
        public static Vector3 operator *(float scalar, Vector3 v) => v * scalar;
        public static Vector3 operator /(Vector3 v, float scalar) => new(v.X / scalar, v.Y / scalar, v.Z / scalar);
        public static float operator *(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;  // Dot product

        public static float Distance(Vector3 a, Vector3 b) => (a - b).Magnitude;

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = t < 0 ? 0 : (t > 1 ? 1 : t);
            return new Vector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }

        public override bool Equals(object obj) => obj is Vector3 v && Equals(v);
        public bool Equals(Vector3 other) => X == other.X && Y == other.Y && Z == other.Z;
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}
