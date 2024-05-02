using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellEjection : MonoBehaviour
{
    Rigidbody rigidBody;
    CapsuleCollider capsuleCollider;
    IEntityAnimable entityAnimable;
    AudioSource audioSource;
    [SerializeField] AudioClip clip;
    private void Awake()
    {
        GameObject foundObject = GameObject.Find("Player");
        entityAnimable = foundObject.GetComponent<IEntityAnimable>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
        rigidBody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        capsuleCollider.enabled = false;
        rigidBody.AddRelativeForce(new Vector3(1 * Random.Range(1f, 1.5f), 0, 1 * Random.Range(1f, 1.5f)) + transform.InverseTransformDirection(entityAnimable.GetLastVelocity()), ForceMode.Impulse);
        Vector3 randomAxis = Random.onUnitSphere;
        float randomSpeed = Random.Range(-10, 10);
        rigidBody.AddRelativeTorque(randomAxis * randomSpeed, ForceMode.Impulse);
        Destroy(gameObject, 2);
        StartCoroutine(ColliderPreparation());
    }
    IEnumerator ColliderPreparation()
    {
        yield return new WaitForSeconds(0.5f);
        capsuleCollider.enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        audioSource.clip = clip;
        audioSource.spatialBlend = 1;
        audioSource.volume = 0.3f;
        audioSource.Play();
    }
}