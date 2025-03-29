using UnityEngine;

public class PassivItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassivItemScriptableObject passivItemData;

    protected virtual void ApplyModifier()
    {
        //  Apply the boost value to the apprpriate stat in the child classes
    }
    void Start()
    {
        player = FindAnyObjectByType<PlayerStats>();
        ApplyModifier();
    }
}
