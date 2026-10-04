using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    [SerializeField] private List<string> _lines;
    
    public List<string> Lines
    {
        get { return _lines; }
    }
}
