using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityMeleeAttack : MonoBehaviour
{
    [SerializeField] Vector3 offset = Vector3.forward + Vector3.up;
    [SerializeField] float radius = 0.35f;
    [SerializeField] LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] string[] affectedTags = { "Untagged", "Player" };
    [SerializeField] float damage = 1;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clipsMiss;
    [SerializeField] AudioClip[] clipsHit;
    [Header("Debug")]
    [SerializeField] bool debugAttack;

    private void OnValidate()
    {
        if (debugAttack)
        {
            debugAttack = false;
            PerformAttack();
        }
    }
    public void PerformAttack()
    {
        int random;
        Collider[] potentialHurtboxes = Physics.OverlapSphere(CalculateAttackPosition(), radius, layerMask);
        foreach(Collider c in potentialHurtboxes)
        {
            if(affectedTags.Contains(c.tag))
            {
                random = Random.Range(0, clipsMiss.Length);
                audioSource.clip = clipsMiss[random];
                audioSource.Play();
                Hurtbox hurtbox = c.GetComponentInParent<Hurtbox>();
                hurtbox?.NotifyHit(this, damage);
                return;
            }
        }
        random = Random.Range(0, clipsMiss.Length);
        audioSource.clip = clipsHit[random];
        audioSource.Play();
    }
    Vector3 CalculateAttackPosition()
    {
        return transform.position + transform.TransformDirection(offset);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 gizmoPos = transform.position+ transform.TransformDirection(offset);
        Gizmos.DrawWireSphere(gizmoPos, radius);
    }
}
