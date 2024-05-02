using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : MonoBehaviour
{
    SphereCollider sCollider;
    [SerializeField] int weaponKind;
    [SerializeField] int ammoAmount;
    AudioSource audioSource;
    public bool enablePickUp;
    private void Awake()
    {
        audioSource = GameObject.Find("GrabAmmoSound").GetComponent<AudioSource>();
        sCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {
        if (enablePickUp)
        {
            sCollider.enabled = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && enablePickUp)
        {
            audioSource.Play();
            switch (weaponKind)
            {
                case 0:
                    GameManager.Instance.bufferAmmoUSP += ammoAmount;
                    break;
                case 1:
                    GameManager.Instance.bufferAmmoM4 += ammoAmount;
                    break;
                case 2:
                    GameManager.Instance.bufferAmmoSG += ammoAmount;
                    break;
            }
            other.GetComponentInChildren<BarrelByRaycast>().UpdateAmmoUI();
            Destroy(gameObject);
        }
    }
}