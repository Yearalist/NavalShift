using UnityEngine;

public class Mermi : MonoBehaviour
{
    public float speed = 20f; // Mermi hızı
    public float damage = 1f; // Hasar miktarı
    private Transform target; // Hedef

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; // Hedefi ayarla
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Hedef yoksa mermiyi yok et
            return;
        }

        // Hedefe doğru hareket et
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Eğer hedefe yaklaştıysa hasar ver ve mermiyi yok et
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            BotVuruldu enemy = target.GetComponent<BotVuruldu>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Hedefe hasar ver
            }
            Destroy(gameObject); // Mermiyi yok et
        }
    }
}
