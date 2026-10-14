using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Infrastructure
{
    public class GameStateService : IGameStateService
    {
        public GameState Current { get; private set; } = GameState.FreeRoam;
        public event Action<GameState> StateChanged;

        public void Set(GameState state)
        {
            if (state == Current)
                return;
            Current = state;
            StateChanged?.Invoke(state);
        }
    }
}
