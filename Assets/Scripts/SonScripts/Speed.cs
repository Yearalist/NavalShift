using UnityEngine;


public class Speed : MonoBehaviour
{
    public float initialSpeed = 1f; // Ba�lang�� h�z�
    public float acceleration = 0.1f; // �vme miktar�
    public float raycastDistance = 2f; // Raycast mesafesi
    public LayerMask islandLayer; // Adac�k i�in layer mask

    private float currentSpeed; // �u anki h�z
    private bool stopMovement = false;

    void Start()
    {
        // Ba�lang�� h�z�n� ayarla
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!stopMovement) // E�er hareket durdurulmad�ysa
        {
            // H�z� art�r
            currentSpeed += acceleration * Time.deltaTime;

            // Nesneyi X ekseninde hareket ettir
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
        }

        // Raycast kontrol�
        CheckForIsland();
    }

    void CheckForIsland()
    {
        // Botun �n�ne raycast at
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.forward, out hit, raycastDistance, islandLayer))
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
       
    }
  
    public void ResumeBot()
    {
        stopMovement = false; // Hareketi yeniden ba�lat
    }

    void OnDrawGizmos()
    {
        // Raycast �izgilerini g�rselle�tir
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -Vector3.forward * raycastDistance);
    }
}