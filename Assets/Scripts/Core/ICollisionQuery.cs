using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface ICollisionQuery
    {
        public bool IsWalkable(Vector2 position);
        public IInteractable FindInteractable(Vector2 position);
    }
}
