using Assets.Scripts.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class ZoneDetector
    {
        private readonly Transform _transform;
        private readonly LayerMask _zoneLayer;
        private ITriggerZone _current;
        public event Action<ITriggerZone> OnEntered;
        public event Action<ITriggerZone> OnExited;

        public ZoneDetector(Transform transform, LayerMask zoneLayer)
        {
            _transform = transform;
            _zoneLayer = zoneLayer;
        }

        public void Tick()
        {
            var hit = Physics2D.OverlapCircle(_transform.position, 0.5f, _zoneLayer);
            ITriggerZone zone = null;

            if (hit != null)
                hit.TryGetComponent(out zone);

            if (zone == _current)
                return;

            if (_current != null)
            {
                _current.OnExit();
                OnExited?.Invoke(_current);
            }
            _current = zone;

            if (_current != null)
            {
                _current.OnEnter();
                OnEntered?.Invoke(_current);
            }
        }
    }
}
