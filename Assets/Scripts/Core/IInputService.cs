using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IInputService
    {
        public Vector2 Move { get; }
        public bool InteractPressed { get; }
        public bool CancelPressed { get; }
        public bool BackPressed { get; }
    }
}
