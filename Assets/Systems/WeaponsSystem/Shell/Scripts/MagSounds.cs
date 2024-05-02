using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagSounds : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] Collider[] colliders;
    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        foreach(Collider c in colliders)
        {
            c.enabled = false;
        }
        Vector3 randomAxis = Random.onUnitSphere;
        float randomSpeed = Random.Range(-10, 10);
        rigidBody.AddRelativeTorque(randomAxis * randomSpeed, ForceMode.Impulse);
        Destroy(gameObject, 2);
        StartCoroutine(ColliderPreparation());
    }
    IEnumerator ColliderPreparation()
    {
        yield return new WaitForSeconds(0.2f);
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
    }
}
