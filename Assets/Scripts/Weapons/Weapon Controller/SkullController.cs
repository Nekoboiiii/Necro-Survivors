using UnityEngine;

[System.Obsolete("Will be replaced by the new System")]
public class SkullController : WeaponController
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedSkull = Instantiate(weaponData.Prefab, transform.position, Quaternion.identity);
        SkullBehaviour skullBehaviour = spawnedSkull.GetComponent<SkullBehaviour>();

        if (skullBehaviour != null)
        {
            // Use the player's movement direction if moving, otherwise use lastMovedVector
            Vector2 shootDirection = playerMovement.moveDir;

            // If the player is not moving, use the last moved direction
            if (shootDirection == Vector2.zero && playerMovement.lastMovedVector != Vector2.zero)
            {
                shootDirection = playerMovement.lastMovedVector;
            }

            // Pass the final direction to the SkullBehaviour
            skullBehaviour.DirectionChecker(shootDirection);
        }
    }
}
