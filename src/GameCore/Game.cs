using System;
using System.Collections.Generic;

namespace GameCore
{
    public class Game
    {
        private readonly List<Entity> _entities = new();
        private readonly List<ISystem> _systems = new();
        private readonly IEngineInput _input;

        public IReadOnlyList<Entity> Entities => _entities;

        public Game(IEngineInput input)
        {
            _input = input;
        }

        public Entity CreateEntity()
        {
            var entity = new Entity(_entities.Count + 1);
            _entities.Add(entity);
            return entity;
        }

        public void AddSystem(ISystem system)
        {
            _systems.Add(system);
        }

        public void Update(float deltaTime)
        {
            HandleInput(deltaTime);

            foreach (var system in _systems)
            {
                system.Update(deltaTime, _entities);
            }
        }

        private void HandleInput(float deltaTime)
        {
            if (_input.IsKeyDown("Space"))
            {
                // Example action hook for the engine integration
            }
        }
    }
}
