using UnityEngine;

public class Tower : MonoBehaviour
{
    public float attackRange = 10f; // Kule menzili
    public float fireRate = 1f; // Ateş etme hızı (saniye cinsinden)
    public GameObject projectilePrefab; // Mermi prefab'ı
    public Transform firePoint; // Ateş noktası
    
    public AudioSource audioSource; // Ses kaynağı
    public AudioClip fireSound; // Ateş sesi

    public Transform currentTarget; // Mevcut hedef
    private float fireCooldown = 0f; // Ateş gecikmesi

    void Update()
    {
        // Ateş gecikmesini azalt
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

        // Hedefi bul ve saldır
            FindTarget();
        if (currentTarget != null && fireCooldown <= 0)
        {
            Shoot(currentTarget);
            fireCooldown = fireRate; // Ateş hızını sıfırla
        }
    }

    void FindTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                currentTarget = hitCollider.transform;
                return; // Bir hedef bulunca çık
            }
        }

        currentTarget = null; // Eğer hedef yoksa sıfırla
    }

    void Shoot(Transform target)
    {
        if (target == null)
        {
            return; // Hedef yoksa çık
        }

        // Mermi oluştur
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Mermi scriptine hedefi gönder
        Mermi mermiScript = projectile.GetComponent<Mermi>();
        if (mermiScript != null)
        {
            mermiScript.SetTarget(target);
            
        }
        
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }

    // Kule menzilini görselleştirmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
