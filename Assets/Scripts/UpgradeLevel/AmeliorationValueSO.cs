using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AmeliorationValueSO : ScriptableObject
{
    public List<LevelMiniGameValue> levelsValue;
}

[Serializable]
public class LevelMiniGameValue
{
    public int value;
    public int cooldown;
    public int miniSheep;
    public int maxiSheep;
}