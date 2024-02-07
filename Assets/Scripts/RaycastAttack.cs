using Fusion;
using UnityEngine;
public class RaycastAttack : NetworkBehaviour
{
    public float Damage = 10;


    public void ShootRayAttack(Vector3 origin, Vector3 direction)
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        var ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);
        if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out var hit))
        {
            if (hit.transform.TryGetComponent<PlayerHealth>(out var health))
            {
                if (health.Id != Id)
                {

                    health.DealDamageRpc(Object.GetBehaviour<NetworkBehaviour>(),Damage);

                }
            }
        }

    }
}
