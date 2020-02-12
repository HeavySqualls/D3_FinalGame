using UnityEngine;
using Kryz.CharacterStats;

public class StatPanel : MonoBehaviour
{
    [SerializeField] StatDisplay[] statDisplays;
    [SerializeField] string[] statNames;

    private CharacterStat[] stats;

    private void OnValidate()
    {
        statDisplays = GetComponentsInChildren<StatDisplay>();
        UpdateStatNames();
    }

    // Recieves stats 
    public void SetStats(params CharacterStat[] charStats)
    {
        stats = charStats;

        // Check if we have more stats than stat displays 
        if (stats.Length > statDisplays.Length)
        {
            Debug.LogError("Not Enough Stat Displays!");
            return;
        }

        // if we have more stat displays than stats, disable the extra stat displays 
        for (int i = 0; i < statDisplays.Length; i++)
        {
            statDisplays[i].gameObject.SetActive(i < stats.Length);

            if (i < stats.Length)
            {
                statDisplays[i].Stat = stats[i];
            }
        }
    }

    // Iterates through stats and sets the corresponding stat value text to the current stat value
    public void UpdateStatValues() // called from the stats manager class when ever we need to update stats 
    {
        for (int i = 0; i < stats.Length; i++)
        {
            statDisplays[i].UpdateStatValue();
        }
    }

    // Iterates through stats and sets the corresponding name text to the current stat name
    public void UpdateStatNames() // called from the stats manager class when ever we need to update stats 
    {
        for (int i = 0; i < statNames.Length; i++)
        {
            statDisplays[i].Name = statNames[i]; 
        }
    }
}
