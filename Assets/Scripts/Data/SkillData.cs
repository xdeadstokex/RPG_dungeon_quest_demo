using Assets.Scripts.Battle;
using System;
using UnityEngine;


[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData :ScriptableObject
{
    [SerializeField] private string _skillName = "Attack";
    [SerializeField, Min(0)] private int _mpCost = 0;
    [SerializeField, Min(0f)] private float _power = 1f;
    [SerializeField] private TargetType _target = TargetType.SingleEnemy;
    [SerializeField] private CommandKind _command = CommandKind.Timing;
    [SerializeField, Min(0.2f)] private float _commandDuration = 1f;
    [SerializeField, Range(0.05f, 0.5f)] private float _goodWindow = 0.25f;
    [SerializeField, Range(0.02f, 0.2f)] private float _missWindow = 0.1f;
    [SerializeField, Range(0.02f, 0.2f)] private float _perfectWindow = 0.08f;
    [SerializeField] private int _mpGain = 1;

    public string SkillName => _skillName;
    public int MpCost => _mpCost;
    public float Power => _power;
    public TargetType Target => _target;
    public CommandKind Command => _command;
    public float CommandDuration => _commandDuration;
    public float GoodWindow => _goodWindow;
    public float MissWindow => _missWindow;
    public float PerfectWindow => _perfectWindow;
    public int MpGain => _mpGain;
}
