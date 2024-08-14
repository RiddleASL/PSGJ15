using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilityHolder : MonoBehaviour
{
    PlayerMotor pm;

    public Ability primaryAbility;
    float primaryCooldown;

    public Ability secondaryAbility;
    float secondaryCooldown;

    public Ability passiveAbility;
    bool passiveActive;

    // Start is called before the first frame update
    void Start()
    {
        pm = gameObject.GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        handleCooldowns();
        Debug.Log("Primary Cooldown: " + primaryCooldown + " Secondary Cooldown: " + secondaryCooldown);
        if(passiveAbility != null && !passiveActive)
        {
            passiveAbility.Activate(gameObject);
            passiveActive = true;
        }
    }

    public void UsePrimaryAbility(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if(primaryCooldown > 0) return;
        Debug.Log("Primary Ability Used");
        primaryAbility.Activate(gameObject);
        primaryCooldown = primaryAbility.cooldown;
    }

    public void UseSecondaryAbility(InputAction.CallbackContext context)
    {
        if(secondaryCooldown > 0 || !context.performed) return;
        Debug.Log("Secondary Ability Used");
        secondaryAbility.Activate(gameObject);
        secondaryCooldown = secondaryAbility.cooldown;
    }

    public void ResetChar(){
        primaryCooldown = 0;
        secondaryCooldown = 0;
        passiveActive = false;
    }

    void handleCooldowns()
    {
        if(primaryCooldown > 0 && pm.primaryAbilityDuration <= 0){primaryCooldown -= Time.deltaTime;}
        if(secondaryCooldown > 0 && pm.secondaryAbilityDuration <= 0){secondaryCooldown -= Time.deltaTime;}
    }

    //public setters
    public void setPrimaryCooldown(float cooldown) {primaryCooldown = cooldown;}
    public void setSecondaryCooldown(float cooldown) {secondaryCooldown = cooldown;}
}
