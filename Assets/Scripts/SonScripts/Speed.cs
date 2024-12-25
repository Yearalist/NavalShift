using UnityEngine;


public class Speed : MonoBehaviour
{
    public float initialSpeed = 1f; // Baþlangýç hýzý
    public float acceleration = 0.1f; // Ývme miktarý
    public float raycastDistance = 2f; // Raycast mesafesi
    public LayerMask islandLayer; // Adacýk için layer mask

    private float currentSpeed; // Þu anki hýz
    private bool stopMovement = false;

    void Start()
    {
        // Baþlangýç hýzýný ayarla
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (!stopMovement) // Eðer hareket durdurulmadýysa
        {
            // Hýzý artýr
            currentSpeed += acceleration * Time.deltaTime;

            // Nesneyi X ekseninde hareket ettir
            transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
        }

        // Raycast kontrolü
        CheckForIsland();
    }

    void CheckForIsland()
    {
        // Botun önüne raycast at
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.forward, out hit, raycastDistance, islandLayer))
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
       
    }
  
    public void ResumeBot()
    {
        stopMovement = false; // Hareketi yeniden baþlat
    }

    void OnDrawGizmos()
    {
        // Raycast çizgilerini görselleþtir
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + -Vector3.forward * raycastDistance);
    }
}