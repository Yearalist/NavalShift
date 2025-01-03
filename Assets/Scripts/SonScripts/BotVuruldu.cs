using UnityEngine;
using System.Collections; // IEnumerator i�in gerekli namespace

public class BotVuruldu : MonoBehaviour
{
    public float asagiHareket = 2f; // A�a�� hareket h�z�
    public float hareketSuresi = 4f; // Hareket s�resi

    private bool hareketEtmelimi = false; // Hareket kontrol�

    private void Update()
    {
        if (hareketEtmelimi)
        {
            // Gemiyi a�a�� hareket ettir
            transform.Translate(Vector3.down * asagiHareket * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            hareketEtmelimi = true;
            Invoke("DestroyBot", hareketSuresi);
        }
    }

    private void DestroyBot()
    {
        Destroy(gameObject);
    }
}
