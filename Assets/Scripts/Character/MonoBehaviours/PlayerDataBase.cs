using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase", menuName = "Data", order = 1)]
public class PlayerDataBase : ScriptableObject
{
    public enum AbilityType { NONE, DASH, GIMMICK_ACTIVATE }

    public List<AbilityType> abilityTypes;
    public Transform transform;
    public PlayerInput playerInput;
    public Damageable damageable;


}
