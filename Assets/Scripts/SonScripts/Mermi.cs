using UnityEngine;

public class Mermi : MonoBehaviour
{
    public float speed = 20f; // Mermi hızı
    private Transform target; // Hedef

    public void SetTarget(Transform newTarget)
    {
        target = newTarget; // Hedefi ayarla
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Hedef yoksa kendini yok et
            return;
        }

        // Hedefe doğru hareket et
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Eğer hedefe çok yaklaştıysa yok et
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            Destroy(target.gameObject); // Hedefi yok et
            Destroy(gameObject); // Kendini yok et
        }
    }
}
