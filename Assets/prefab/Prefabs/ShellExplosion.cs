using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;
    public AudioSource m_ExplosionAudio;
    public float m_MaxDamage = 100f;
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 10f;
    public float m_ExplosionRadius = 5f;

    private void Start()
    {
        // Eðer yok edilmediyse, mermiyi belli bir süre sonra yok et
      //  Destroy(gameObject, m_MaxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Patlama alanýndaki tüm collider'larý topla
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;

            // Patlama kuvveti uygula
            targetRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            targetRigidbody.AddExplosionForce(m_ExplosionForce * 0.5f, transform.position, m_ExplosionRadius);
        }

        // Parçacýklarý shell'den ayýr ve çalýþtýr
      //  m_ExplosionParticles.transform.parent = null;
      //  m_ExplosionParticles.Play();

        // Ses efektini çal
      //  m_ExplosionAudio.Play();

        // Parçacýklarý yok et
       // ParticleSystem.MainModule mainModule = m_ExplosionParticles.main;
       // Destroy(m_ExplosionParticles.gameObject, mainModule.duration);

        // Shell'i yok et
        Destroy(gameObject,10f);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float explosionDistance = explosionToTarget.magnitude;
        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;
        float damage = Mathf.Max(0f, relativeDistance * m_MaxDamage);
        return damage;
    }
}
