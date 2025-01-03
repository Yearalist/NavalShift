using UnityEngine;

public class Speed : MonoBehaviour
{
    public float initialSpeed = 1f; // Baþlangýç hýzý
    public float acceleration = 0.1f; // Ývme miktarý
    public float maxSpeed = 5f; // Maksimum hýz sýnýrý
    public float raycastDistance = 2f; // Raycast mesafesi
    public LayerMask islandLayer; // Adacýk için layer mask

    private float currentSpeed; // Þu anki hýz
    private bool stopMovement = false;
    private float raycastCheckInterval = 0.1f; // Raycast kontrol sýklýðý (0.1 saniye)
    private float lastRaycastCheckTime = 0f; // Son raycast kontrol zamaný

    void Start()
    {
        // Baþlangýç hýzýný ayarla
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!stopMovement) // Eðer hareket durdurulmadýysa
        {
            // Hýzý artýr (maksimum hýzý aþma)
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

            // Nesneyi X ekseninde hareket ettir
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
        }

        // Performans iyileþtirme: Raycast kontrolünü belirli aralýklarla yap
        if (Time.time - lastRaycastCheckTime >= raycastCheckInterval)
        {
            lastRaycastCheckTime = Time.time;
            if (!stopMovement) // Sadece hareket ederken raycast kontrolü yap
            {
                CheckForIsland();
            }
        }
    }

    void CheckForIsland()
    {
        // Botun önüne raycast at
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.left, out hit, raycastDistance, islandLayer))
        {
            // Eðer raycast bir adaya çarparsa hareketi durdur
            if (!stopMovement)
            {
                Debug.Log("Bot bulundu, hareket durduruldu.");
                StopBot();
            }
        }
        else
        {
            // Eðer ada yoksa hareketi yeniden baþlat
            if (stopMovement)
            {
                Debug.Log("Bot yok, hareket devam ediyor.");
                ResumeBot();
            }
        }
    }

    public void StopBot()
    {
        stopMovement = true; // Hareketi durdur
        currentSpeed = 0f; // Hýzý sýfýrla
    }

    public void ResumeBot()
    {
        stopMovement = false; // Hareketi yeniden baþlat
        currentSpeed = initialSpeed; // Hýzý baþlangýç hýzýna ayarla
    }

    void OnDrawGizmos()
    {
        // Raycast çizgilerini görselleþtir
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * raycastDistance);
    }
}
