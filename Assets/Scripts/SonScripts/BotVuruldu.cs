using UnityEngine;

public class BotVuruldu : MonoBehaviour
{
    public float asagiHareket = 2f;
    private bool hareketEtmelimi = false;
    private float BotuYokEt = 1f;

    private void Update()
    {
        // Eðer hareket etmesi gerekiyorsa küpün -y yönünde hareketini saðla
       /* if (hareketEtmelimi)
        {
            transform.Translate(Vector3.down * asagiHareket * Time.deltaTime);
        }
    */
        }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
        {
            hareketEtmelimi = true;
            Destroy(gameObject, BotuYokEt); // 4 saniye sonra yok et
        }
    }
}