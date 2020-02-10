using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class StatsManager : MonoBehaviour
{
    public CharacterStat strength;
    public CharacterStat agility;
    public CharacterStat intellect;
    public CharacterStat vitality;

    [SerializeField] StatPanel statPanel;

    private void Awake()
    {
        statPanel.SetStats(strength, agility, intellect, vitality);
        statPanel.UpdateStatValues();
    }
}
