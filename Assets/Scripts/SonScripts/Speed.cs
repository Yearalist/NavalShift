using UnityEngine;

public class Speed : MonoBehaviour
{
    public float initialSpeed = 1f; // Ba�lang�� h�z�
    public float acceleration = 0.1f; // �vme miktar�
    public float maxSpeed = 5f; // Maksimum h�z s�n�r�
    public float raycastDistance = 2f; // Raycast mesafesi
    public LayerMask islandLayer; // Adac�k i�in layer mask

    private float currentSpeed; // �u anki h�z
    private bool stopMovement = false;
    private float raycastCheckInterval = 0.1f; // Raycast kontrol s�kl��� (0.1 saniye)
    private float lastRaycastCheckTime = 0f; // Son raycast kontrol zaman�

    void Start()
    {
        // Ba�lang�� h�z�n� ayarla
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!stopMovement) // E�er hareket durdurulmad�ysa
        {
            // H�z� art�r (maksimum h�z� a�ma)
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

            // Nesneyi X ekseninde hareket ettir
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
        }

        // Performans iyile�tirme: Raycast kontrol�n� belirli aral�klarla yap
        if (Time.time - lastRaycastCheckTime >= raycastCheckInterval)
        {
            lastRaycastCheckTime = Time.time;
            if (!stopMovement) // Sadece hareket ederken raycast kontrol� yap
            {
                CheckForIsland();
            }
        }
    }

    void CheckForIsland()
    {
        // Botun �n�ne raycast at
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.left, out hit, raycastDistance, islandLayer))
        {
            // E�er raycast bir adaya �arparsa hareketi durdur
            if (!stopMovement)
            {
                Debug.Log("Bot bulundu, hareket durduruldu.");
                StopBot();
            }
        }
        else
        {
            // E�er ada yoksa hareketi yeniden ba�lat
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
        currentSpeed = 0f; // H�z� s�f�rla
    }

    public void ResumeBot()
    {
        stopMovement = false; // Hareketi yeniden ba�lat
        currentSpeed = initialSpeed; // H�z� ba�lang�� h�z�na ayarla
    }

    void OnDrawGizmos()
    {
        // Raycast �izgilerini g�rselle�tir
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * raycastDistance);
    }
}
