using UnityEngine;

public class Tower : MonoBehaviour
{
    public float attackRange = 10f; // Kule menzili
    public float fireRate = 1f; // Ateş etme hızı (saniye cinsinden)
    public GameObject projectilePrefab; // Mermi prefab'ı
    public Transform firePoint; // Ateş noktası

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

        // Mermi yönü
        Vector3 direction = (target.position - firePoint.position).normalized;

        // Mermi hızını ayarla
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * 10f; // Hızı 10 olarak belirledik
        }
    }

    // Kule menzilini görselleştirmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
