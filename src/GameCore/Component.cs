namespace GameCore
{
    public abstract class Component
    {
        public Entity Owner { get; internal set; }
    }

    public class TransformComponent : Component
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Rotation { get; set; }
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
    }

    public class SpriteComponent : Component
    {
        public string SpriteId { get; set; }
        public float Opacity { get; set; } = 1.0f;
    }

    public class RigidbodyComponent : Component
    {
        public float VelX { get; set; }
        public float VelY { get; set; }
        public float Mass { get; set; } = 1.0f;
    }
