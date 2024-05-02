using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickUpWeapon : MonoBehaviour
{
    [SerializeField] int weaponIndex;
    [SerializeField] bool isHealing;
    [SerializeField] GameObject[] sound;
    public UnityEvent dismiss;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        StartCoroutine(TimeToDismiss());
    }
    IEnumerator TimeToDismiss()
    {
        yield return new WaitForSeconds(4);
        animator.SetBool("Dismissing", true);
        yield return new WaitForSeconds(2);
        StartCoroutine(Dismissing());
    }
    IEnumerator Dismissing()
    {
        yield return new WaitForSeconds(6);
        dismiss.Invoke();
        Destroy(gameObject);
    }
    public void Detatch() 
    {
        transform.parent = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<EntityLife>().OnHitNotifiedWithOffender(-5, transform);
            Instantiate(sound[0], transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
