using System;
using UnityEngine;

/// <summary>
/// Dynamic data describing a commander participant in a poker battle.
/// </summary>
[Serializable]
public struct PlayerData
{
    public Sprite Profile;
    public string Name;
    public int Id;
    public int Currency;
    public bool ActiveInRound;
    public BetState BetState;
    public int BetThisRound;
    public IPlayerLogic PlayerLogic;
}
    