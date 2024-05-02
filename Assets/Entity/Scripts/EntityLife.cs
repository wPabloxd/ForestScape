using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class EntityLife : MonoBehaviour
{
    public UnityEvent<Vector3> died;
    public UnityEvent spawnHeal;
    [Header("Stats")]
    [SerializeField] float maxLife = 3f;
    [SerializeField] float minDeathPushForce = 2000;
    [SerializeField] float maxDeathPushForce = 3000;
    public float currentLife;

    [Header("Audio")]
    AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] bool isPlayer;

    [Header("Debug")]
    [SerializeField] bool debugDamage;

    bool healingCooldown;
    float timeLeft;
    HPBar hpBar;
    Animator anim;
    NavMeshAgent agent;
    Hurtbox hurtbox;
    Enemy enemy;
    AIDecisionMaker aIDecisionMaker;
    EntityRagdollizer entityRagdollizer;
    PlayerController playerController;
    CharacterController characterController;
    private void Awake()
    {
        aIDecisionMaker = GetComponent<AIDecisionMaker>();
        audioSource = GetComponent<AudioSource>();
        hpBar = GetComponentInChildren<HPBar>();
        currentLife = maxLife;
        enemy = GetComponent<Enemy>();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        hurtbox = GetComponent<Hurtbox>();
        anim = GetComponentInChildren<Animator>();
        entityRagdollizer = GetComponentInChildren<EntityRagdollizer>();
    }
    private void OnEnable()
    {
        hurtbox.onHitNotifiedWithOffender.AddListener(OnHitNotifiedWithOffender);
    }
    private void OnValidate()
    {
        if (debugDamage)
        {
            debugDamage = false;
            //OnHitNotifiedWithOffender(transform);
        }
    }
    private void Update()
    {
        if (currentLife != maxLife)
        {
            if(timeLeft >= 5f)
            {
                timeLeft = 0;
                StartCoroutine(Healing());
            }
            timeLeft += Time.deltaTime;
        }
    }
    public void OnHitNotifiedWithOffender(float damage, Transform offender)
    {
        timeLeft = 0;
        StartCoroutine(ResetCD());
        if (currentLife > 0)
        {
            currentLife -= damage;
            if (isPlayer)
            {
                HealSpawnProbability();
            }
            int randomInt = UnityEngine.Random.Range(0, audioClips.Length);
            hpBar.SetNormalizedValue(Mathf.Clamp01(currentLife / maxLife), (currentLife / maxLife) * 100);
            if (isPlayer && damage > 0)
            {
                audioSource.clip = audioClips[randomInt];
                audioSource.Play();
            }
            if (!isPlayer && !aIDecisionMaker.playerDetected)
            {
                Died(offender);
                hpBar.SetNormalizedValue(Mathf.Clamp01(0), currentLife / maxLife * 100);
            }
            if (currentLife <= 0)
            {
                Died(offender);
            }
        }  
    }
    void Died(Transform offender)
    {
        int randomInt = UnityEngine.Random.Range(0, audioClips.Length);
        if (characterController)
        {
            characterController.enabled = false;
        }
        if (playerController)
        {
            playerController.enabled = false;
        }
        if (agent)
        {
            agent.enabled = false;
        }
        if (enemy)
        {
            enemy.enabled = false;
        }
        anim.enabled = false;

        entityRagdollizer.Ragdollize();
        entityRagdollizer.Push(transform.position - offender.position, minDeathPushForce, maxDeathPushForce);
        died.Invoke(transform.position);
        if (!isPlayer)
        {
            int spawnHealRandom = UnityEngine.Random.Range(0, 100);
            if (spawnHealRandom < GameManager.Instance.spawnHealingProbability)
            {
                spawnHeal.Invoke();
            }
            audioSource.clip = audioClips[randomInt];
            audioSource.Play();
            Destroy(gameObject, 3);
        }
    }
    IEnumerator ResetCD()
    {
        healingCooldown = true;
        yield return new WaitForEndOfFrame();
        healingCooldown = false;
    }
    IEnumerator Healing()
    {
        while (currentLife < maxLife && !healingCooldown)
        {
            if (healingCooldown)
            {
                yield break;
            }
            currentLife += maxLife / 30 * Time.deltaTime;
            hpBar.SetNormalizedValue(Mathf.Clamp01(currentLife / maxLife), (currentLife / maxLife) * 100);
            HealSpawnProbability();
            yield return null;
        }
    }
    void HealSpawnProbability()
    {
        if (currentLife == 10)
        {
            GameManager.Instance.spawnHealingProbability = 0;
        }
        else if (currentLife > 7.5f)
        {
            GameManager.Instance.spawnHealingProbability = 2;
        }
        else if (currentLife > 5)
        {
            GameManager.Instance.spawnHealingProbability = 5;
        }
        else if (currentLife > 2.5f)
        {
            GameManager.Instance.spawnHealingProbability = 10;
        }
        else if (currentLife > 0f)
        {
            GameManager.Instance.spawnHealingProbability = 15;
        }
    }
    private void OnDisable()
    {
        hurtbox.onHitNotifiedWithOffender.RemoveListener(OnHitNotifiedWithOffender);
    }
}