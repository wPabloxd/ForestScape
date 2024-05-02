using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EntityWeapons : MonoBehaviour
{
    [Header("Parents")]
    [SerializeField] Transform weaponsParent;
    [SerializeField] TwoBoneIKConstraint rightHand;
    [SerializeField] TwoBoneIKConstraint leftHand;
    [SerializeField] RigBuilder rigBuilder;
    [SerializeField] Transform[] rightHandTargets;
    [SerializeField] Transform[] leftHandTargets;

    protected Weapon[] weapons;

    [Header("SoundFXs")]
    [SerializeField] AudioClip[] unholster;

    AudioSource audioSource;
    public int currentWeapon = -1;
    bool initialitated;
    bool alreadySwapping;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        weapons = weaponsParent.GetComponentsInChildren<Weapon>();
        currentWeapon = weapons.Length > 0 ? 0 : -1;
        SetCurrentWeapon(currentWeapon);
    }
    public virtual void SetCurrentWeapon(int selectedWeapon)
    {
        GetCurrentWeapon().swapping = false;
        if (selectedWeapon == currentWeapon && initialitated)
        {
            return;
        }
        if (!alreadySwapping && initialitated)
        {
            StartCoroutine(SwapWeapons(selectedWeapon));
        }
        else
        {
            for (int i = 0; i < weapons.Length; i++)
            {
                weapons[i].gameObject.SetActive(i == selectedWeapon);
            }
            initialitated = true;
            rightHand.data.target = rightHandTargets[selectedWeapon];
            leftHand.data.target = leftHandTargets[selectedWeapon];
            rigBuilder.Build();
            currentWeapon = selectedWeapon;
        }
    }
    IEnumerator SwapWeapons(int selectedWeapon)
    {
        alreadySwapping = true;
        GetCurrentWeapon().swapping = true;
        yield return new WaitForSeconds(0.25f);
        int randomIndex = UnityEngine.Random.Range(0, unholster.Length);
        audioSource.clip = unholster[randomIndex];
        audioSource.Play();
        yield return new WaitForSeconds(0.25f);
        GetCurrentWeapon().swapping = false;
        alreadySwapping = false;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == selectedWeapon);
        }
    
        rightHand.data.target = rightHandTargets[selectedWeapon];
        leftHand.data.target = leftHandTargets[selectedWeapon];
        rigBuilder.Build();
        currentWeapon = selectedWeapon;
    }
    public void SelectNextWeapon()
    {
        int nextWeapon = currentWeapon + 1;
        if (nextWeapon >= 3)
        {
            nextWeapon = 0;
        }
        SetCurrentWeapon(nextWeapon);
    }
    public void SelectPreviousWeapon()
    {
        int nextWeapon = currentWeapon - 1;
        if (nextWeapon < 0)
        {
            nextWeapon = 2;
        }
        SetCurrentWeapon(nextWeapon);
    }
    public void Shot()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].Shot();
        }
    }
    public void StartShooting()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].StartShooting();
        }
    }
    public void StopShooting()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].StopShooting();
        }
    }
    public void BurstShooting()
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].BurstShooting();
        }
    }
    internal bool HasCurrentWeapon()
    {
        return currentWeapon != -1;
    }
    internal Weapon GetCurrentWeapon()
    {
        return weapons[currentWeapon];
    }
}