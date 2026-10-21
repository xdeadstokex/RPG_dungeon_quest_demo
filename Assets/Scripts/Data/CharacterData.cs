using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public const int MaxMp = 10;

    [SerializeField] private string _characterName;
    [SerializeField] private Sprite _characterSprite;
    [SerializeField, Min(1)] private int _maxHp = 100;
    [SerializeField, Range(0, MaxMp)] private int _startMp = 0;
    [SerializeField, Min(0)] private int _baseDamage = 10;
    [SerializeField, Min(0)] private int _speed = 10;
    [SerializeField, Min(0)] private int _defense = 5;
    [SerializeField, Range(0f, 1f)] private float _critRate = 0.1f;

    public string CharacterName => _characterName;
    public Sprite CharacterSprite => _characterSprite;
    public int MaxHp => _maxHp;
    public int StartMp => _startMp;
    public int BaseDamage => _baseDamage;
    public int Speed => _speed;
    public int Defense => _defense;
    public float CritRate => _critRate;

}
