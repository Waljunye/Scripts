using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialAbility
{
    None,
    Flying,
    Sacrificial,
    DrawRabbits,
}

[CreateAssetMenu(fileName = "CardInfo", menuName = "Cards/CardInfo", order = 1)]
public class CardInfo : ScriptableObject
{
    [Header("Displayed")]
    public string displayedName;

    [TextArea]
    public string description;

    public Texture texture;

    [Header("Stats")]
    public int cost;
    public int baseAttack;
    public int baseHealth;
    public SpecialAbility ability = SpecialAbility.None;

    [Header("Special")]
    public string specialScriptName;
}
