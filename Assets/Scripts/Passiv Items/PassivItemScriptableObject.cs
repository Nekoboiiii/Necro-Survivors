using UnityEngine;

[CreateAssetMenu(fileName = "PassivItemScriptableObject", menuName = "Scriptable Objects/PassivItem")]
public class PassivItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    public float Multipler {get => multipler; private set => multipler = value; }
    [SerializeField]
    int level; // Not meant to be Modified in the Inspector [Only in the Editor]
    public int Level {get => level; private set => level = value; }
    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab {get => nextLevelPrefab; private set => nextLevelPrefab = value; }
    [SerializeField]
    Sprite icon; // Not meant to be Modified in the Game [Only in the Editor]
    public Sprite Icon {get => icon; private set => icon = value; }
}
