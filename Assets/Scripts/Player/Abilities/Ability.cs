using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    [HideInInspector] public PlayerMotor pm;
    [HideInInspector] public GlobalScript gs;

    public new string name;
    public float cooldown;
    public float duration;

    public virtual void Activate(GameObject parent)
    {
        
    }
}
