using System;
using UnityEngine;

public class BossAttackt : MonoBehaviour
{
    public float attackRange = 10f;
    public float fireRate = 1f;
    public GameObject projecttilePrefab;
    public Transform firePoint;
    public AudioSource audioSource; // Ses kaynağı
    public AudioClip fireSound; // Ateş sesi

    public Transform currentTarget; // Mevcut hedef
    private float fireCooldown = 0f; // Ateş gecikmesi

    private void Update()
    {
        // Ateş gecikmesi azaltılıyor
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        // Hedef seçimi ve ateş etme
        FingTarget();
        if (currentTarget != null && fireCooldown <= 0)
        {
            Shoot(currentTarget);
            fireCooldown = fireRate; // Ateş gecikmesi sıfırlanıyor
        }
    }

    void FingTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        Transform priorityTarget = null;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Kule"))
            {
                priorityTarget = hitCollider.transform;
                break;
            }
            else if (hitCollider.CompareTag("kasaba"))
            {
                priorityTarget = hitCollider.transform;
            }
        }

        currentTarget = priorityTarget;
    }

    void Shoot(Transform target)
    {
        if (target == null)
        {
            return;
        }

        GameObject projectile = Instantiate(projecttilePrefab, firePoint.position + firePoint.forward * 0.5f, Quaternion.identity);


        EnemyMermi enemyMermiScript = projectile.GetComponent<EnemyMermi>();
        if (enemyMermiScript != null)
        {
            enemyMermiScript.SetTarget(target);
        }

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }

    // Moved OnDrawGizmosSelected outside the Shoot method
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
}
