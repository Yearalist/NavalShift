using UnityEngine;

public class MainShellManager : MonoBehaviour
{
       public LayerMask m_TargetMask; // Patlamanın etkilediği katman
    public GameObject m_ExplosionEffect; // Patlama efektleri ve bileşenlerini içeren GameObject
    public float m_MaxDamage = 100f; // Maksimum hasar
    public float m_ExplosionForce = 1000f; // Patlama kuvveti
    public float m_MaxLifeTime = 10f; // Merminin yaşam süresi
    public float m_ExplosionRadius = 5f;

    private void Start()
    {
        // Belirlenen sürede mermiyi yok et
        Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
{
    // Patlama alanındaki collider'ları topla
    Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TargetMask);

    for (int i = 0; i < colliders.Length; i++)
    {
        // Eğer collider bir Rigidbody'ye sahipse, patlama kuvveti uygula
        Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
        if (targetRigidbody != null)
        {
            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);
        }

        // Eğer collider bir BotVuruldu bileşenine sahipse, hasar uygula
        BotVuruldu targetHealth = colliders[i].GetComponent<BotVuruldu>();
        if (targetHealth != null)
        {
            // Hasarı hesapla ve uygula
            float damage = CalculateDamage(colliders[i].transform.position);
            Debug.Log("Damaging: " + targetHealth.name + " with damage: " + damage);
            targetHealth.TakeDamage(damage);
        }
        else
        {
            Debug.Log("No BotVuruldu found on: " + colliders[i].name);
        }
    }

    // Patlama efektini tetikle
    if (m_ExplosionEffect != null)
    {
        // Patlama efektini oluştur
        GameObject explosion = Instantiate(m_ExplosionEffect, transform.position, Quaternion.identity);
        explosion.SetActive(true);

        // Partikül sistemini çalıştır
        ParticleSystem explosionParticles = explosion.GetComponent<ParticleSystem>();
        if (explosionParticles != null)
        {
            explosionParticles.Play();
        }

        // Patlama sesini çal
        AudioSource explosionAudio = explosion.GetComponent<AudioSource>();
        if (explosionAudio != null)
        {
            explosionAudio.Play();
        }

        // Partikül süresi dolduktan sonra patlama GameObject'ini yok et
        ParticleSystem.MainModule mainModule = explosionParticles.main;
        Destroy(explosion, mainModule.duration);
    }

    // Mermiyi yok et
    //Destroy(gameObject);
}

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Patlama merkezinden hedefe olan uzaklığı hesapla
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;

        // Uzaklığa göre hasarı ölçekle
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = Mathf.Max(0f, relativeDistance * m_MaxDamage);

        return damage;
    }
}
