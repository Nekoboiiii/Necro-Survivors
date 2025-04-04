using System;
using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "Scriptable Objects/Character")]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon {get => icon; private set => icon = value; }

    [SerializeField]
    new string name;
    public string Name {get => name; private set => name = value; }

    [SerializeField]
    GameObject startingWeapon;
    public GameObject StartingWeapon {get => startingWeapon; private set => startingWeapon = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth {get => maxHealth; private set => maxHealth = value; }

    [SerializeField]
    float recovery;
    public float Recovery {get => recovery; private set => recovery = value; }

    [SerializeField]
    float moveSpeed;
    public float MoveSpeed {get => moveSpeed; private set => moveSpeed = value; }

    [SerializeField]
    float might;
    public float Might {get => might; private set => might = value; }

    [SerializeField]
    float projectileSpeed;
    public float ProjectileSpeed {get => projectileSpeed; private set => projectileSpeed = value; }
    
    [SerializeField]
    float magnet;
    public float Magnet {get => magnet; private set => magnet = value; }
    
}
