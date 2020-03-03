using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptable Objects", menuName = "DialogueSystem/Character", order = 1)]
public class sCharacter : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
}
