using UnityEngine;
using Ilumisoft.HealthSystem;

public class EnemyDeath : MonoBehaviour
{
    [Header("Налаштування")]
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float destroyDelay = 1f;
    
    [Header("Посилання")]
    [SerializeField] private GameObject capsuleModel;
    [SerializeField] private Collider capsuleCollider;
    
    private Health health;

    void Start()
    {
        if (capsuleModel == null) 
            capsuleModel = transform.parent.gameObject;
        
        if (capsuleCollider == null) 
            capsuleCollider = transform.parent.GetComponent<Collider>();
        
        health = GetComponent<Health>();
        
        if (health != null)
        {
            health.OnHealthEmpty += HandleDeath;
        }
    }

    void HandleDeath()
    {
        if (capsuleModel != null)
        {
            capsuleModel.SetActive(false);
        }
        
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject, destroyDelay);
    }

    void OnDestroy()
    {
        if (health != null)
        {
            health.OnHealthEmpty -= HandleDeath;
        }
    }
}