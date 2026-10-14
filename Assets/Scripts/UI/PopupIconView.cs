using Assets.Scripts.Core;
using Assets.Scripts.Player;
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
            zones.OnExited += _ => _icon.SetActive(true);
            zones.OnExited += _ => _icon.SetActive(false);
        }
    }
}
