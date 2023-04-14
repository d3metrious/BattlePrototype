using UnityEngine;

[CreateAssetMenu(fileName = "Troop", menuName = "ScriptableObjects/CreateNewTroopType", order = 1)]

public class Troop : ScriptableObject
{
    public string TroopName;
    public Sprite Sprite;
    public float Health;
    public float Damage;
    public int Range;

}
