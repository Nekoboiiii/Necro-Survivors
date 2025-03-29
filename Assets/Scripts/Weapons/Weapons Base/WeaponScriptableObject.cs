using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "Scriptable Objects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab{get => prefab; private set => prefab = value; }
    
    // Base Stats
    [SerializeField]
    float damage;
    public float Damage {get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed {get => speed; private set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration {get => cooldownDuration; private set => cooldownDuration = value; }

    [SerializeField]
    int pierce;
    public int Pierce {get => pierce; private set => pierce = value; }

    [SerializeField]
    int level; // Not meant to be Modified in the Game [Only in the Editor]
    public int Level {get => level; private set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab {get => nextLevelPrefab; private set => nextLevelPrefab = value; }

    [SerializeField]
    new string name;
    public string Name {get => name; private set => name = value; }

    [SerializeField]
    string description; // What is the description of this weapon? [if this is an upgrade, place the description of the upgrades]
    public string Description {get => description; private set => description = value; }

    [SerializeField]
    Sprite icon; // Not meant to be Modified in the Game [Only in the Editor]
    public Sprite Icon {get => icon; private set => icon = value; }


}
