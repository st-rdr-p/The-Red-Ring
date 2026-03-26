using System.Collections.Generic;

namespace GameCore
{
    public class Scene
    {
        public string Name { get; }
        public List<Entity> Entities { get; } = new();

        public Scene(string name)
        {
            Name = name;
        }

        public void Add(Entity entity) => Entities.Add(entity);

        public void Remove(Entity entity) => Entities.Remove(entity);
    }
}
