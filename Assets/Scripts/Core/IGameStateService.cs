using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Core
{
    public enum GameState { FreeRoam, Dialog, Battle}
    public interface IGameStateService
    {
        public GameState Current { get; }
        public event Action<GameState> StateChanged;
        public void Set(GameState state);
    }
}
