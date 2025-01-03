using UnityEngine;
using System.Collections; // IEnumerator için gerekli namespace

public class BotVuruldu : MonoBehaviour
{
    public float asagiHareket = 2f; // Aþaðý hareket hýzý
    public float hareketSuresi = 4f; // Hareket süresi

    private bool hareketEtmelimi = false; // Hareket kontrolü

    private void Update()
    {
        if (hareketEtmelimi)
        {
            // Gemiyi aþaðý hareket ettir
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
