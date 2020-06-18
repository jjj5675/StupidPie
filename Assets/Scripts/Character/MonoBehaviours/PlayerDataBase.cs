using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataBase", menuName = "Data", order = 1)]
public class PlayerDataBase : ScriptableObject
{
    public enum AbilityType { NONE, DASH, INTERACTION }

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

    public void SetDate(Transform transform, PlayerInput playerInput, Damageable damageable, Animator animator, Collider2D collider)
    {
        this.transform = transform;
        this.playerInput = playerInput;
        this.damageable = damageable;
        this.animator = animator;
        this.collider = collider;

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
