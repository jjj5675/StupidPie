using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase", menuName = "Data/PlayerData", order = 1)]
public class PlayerDataBase : ScriptableObject
{
    public enum AbilityType { NONE, DASH, INTERACTION, WALL_JUMP}

    public List<AbilityType> abilityTypes;
    [HideInInspector]
    public Transform transform;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public Damageable damageable;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Collider2D collider;
    [HideInInspector]
    public CharacterController2D character;

    public void SetDate(Transform transform, PlayerInput playerInput, Damageable damageable, Animator animator, Collider2D collider, CharacterController2D character)
    {
        this.transform = transform;
        this.playerInput = playerInput;
        this.damageable = damageable;
        this.animator = animator;
        this.collider = collider;
        this.character = character;

        if(abilityTypes.Contains(AbilityType.DASH))
        {
            this.playerInput.Dash.Enable();
        }
        else
        {
            this.playerInput.Dash.Disable();
        }

        if (abilityTypes.Contains(AbilityType.INTERACTION))
        {
            this.playerInput.Interact.Enable();
        }
        else
        {
            this.playerInput.Interact.Disable();
        }
    }
}
