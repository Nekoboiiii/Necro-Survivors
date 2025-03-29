using UnityEngine;

public class SkullBehaviour : ProjectileWeaponBehaviour
{

    public void Initialize(Vector3 dir)
    {
        DirectionChecker(dir);
    }

    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime;
    }
}
