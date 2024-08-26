using System;
using System.Collections;
using System.Collections.Generic;
using CristiEventSystem.EventChannels;
using EventTriggerSystem;
using EventTriggerSystem.EventActions;
using NaughtyAttributes;
using UnityEngine;

public class IHealth : MonoBehaviour
{
    public float startingHealth;
    public float currentHealth;
    public float maxHealth;
    public bool isDead;
    public EventActionSettings eventWhenDead;
    public GameObject healthBarPrefab;
    private GameObject _healthBar;
    private RectTransform _healthBarTransform;
    
    private EnemyEventChannel enemyEventChannel;
    
    
    private void Awake()
    {
        currentHealth = startingHealth;

        if (!healthBarPrefab)
        {
            Debug.LogError("HealthBarPrefab is not set in the inspector");
            // healthBarPrefab = Resources.Load<GameObject>("HealthBar");
            
        }

    }

    private void OnEnable()
    {
        enemyEventChannel.OnEnemyAttack += (damage) => TakeDamage(damage);
    }
    
    private void OnDisable()
    {
        enemyEventChannel.OnEnemyAttack -= (damage) => TakeDamage(damage);
    }

    private void Start()
    {
        if (!_healthBar)
        {
            _healthBar = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);   
            _healthBarTransform = _healthBar.GetComponent<RectTransform>();

        }else if (!_healthBarTransform)
        {
            _healthBarTransform = _healthBar.GetComponent<RectTransform>();
        }
        //_animator.SetBool("IsDead", false);
    }
    
    [Button]
    public void SendEventTest()
    {
        eventWhenDead.Trigger();
    }
    
    [Button]
    private void NukeTower()
    {
        // Not implemented exception
        throw new NotImplementedException();
    }

    
    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }
    
    private void UpdateHealthBar()
    {
        float healthPercentage = currentHealth / maxHealth;
        _healthBarTransform.localScale = new Vector3(healthPercentage, 1, 1);
    }
    
    private void Die()
    {
        isDead = true;
        Destroy(_healthBar);
        Destroy(gameObject);
        
        //_animator.SetBool("IsDead", true);
        GameManager.Instance.gameOverMusic.Post(gameObject);
        // Invoke("SetIsDeadTrue", 3.5f); // Delay the game over screen
        //gameManagerEventChannel.RaiseGameOver();

    }
    
    
}
