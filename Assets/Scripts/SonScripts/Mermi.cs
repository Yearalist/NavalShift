using UnityEngine;

public class Mermi : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject); // Düşmanı yok et
        }
        Destroy(gameObject); // Kendini yok et
    }
}
