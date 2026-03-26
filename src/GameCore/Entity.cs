using System;
using System.Collections.Generic;

namespace GameCore
{
    public class Entity
    {
        public int Id { get; private set; }
        private readonly Dictionary<Type, Component> _components = new();

        public Entity(int id)
        {
            Id = id;
        }

        public T AddComponent<T>(T component) where T : Component
        {
            _components[typeof(T)] = component;
            component.Owner = this;
            return component;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            if (_components.TryGetValue(typeof(T), out var c) && c is T typed)
            {
                component = typed;
                return true;
            }
            component = null;
            return false;
        }

        public void RemoveComponent<T>() where T : Component
        {
            _components.Remove(typeof(T));
        }
    }
}
