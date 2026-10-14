using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    public class PhysicsCollisionQuery : ICollisionQuery
    {
        private readonly LayerMask _solid, _interactive;
        private const float Radius = 0.2f;

        public PhysicsCollisionQuery(LayerMask solid, LayerMask interactive)
        {
            _solid = solid;
            _interactive = interactive;
        }

        public IInteractable FindInteractable(Vector2 position)
        {
            var col = Physics2D.OverlapCircle(position, Radius, _interactive);
            if (col != null && col.TryGetComponent(out IInteractable res))
                return res;
            else
                return null;
        }

        public bool IsWalkable(Vector2 position)
        {
            return Physics2D.OverlapCircle(position, Radius, _solid | _interactive) == null;
        }
    }
}
