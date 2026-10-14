using Assets.Scripts.Core;
using Assets.Scripts.Player;
using Assets.Scripts.Scenes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class PopupIconView : MonoBehaviour
    {
        [SerializeField] private GameObject _icon;


        public void Bind(ZoneDetector zones)
        {
            zones.OnEntered += z => { if (z is PopupZone) _icon.SetActive(true); };
            zones.OnExited += z => { if (z is PopupZone) _icon.SetActive(false); };
        }
    }
}
