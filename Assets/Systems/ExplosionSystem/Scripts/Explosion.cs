using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float range = 5;
    [SerializeField] float force = 1000;
    [SerializeField] float damage = 5;
    [SerializeField] float upwardsModifier = 1000;
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] GameObject visualExplosionPrefab;

    void Start()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, layerMask);
        foreach(Collider c in colliders)
        {
            if(c.TryGetComponent<Hurtbox>(out Hurtbox hb))
            {
                hb.NotifyHit(this, damage);
            }
            if(c.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(force, transform.position, range, upwardsModifier);
            }
        }
        Instantiate(visualExplosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
