using UnityEngine;
using UnityEngine.UI;

public class BotVuruldu : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f; // Maksimum can
    private float currentHealth; // Mevcut can

    [SerializeField] private Image healthBarSprite; // Saðlýk barý görseli
    [SerializeField] private Transform healthBarTransform; // Saðlýk barýnýn transformu
    [SerializeField] private float reduceSpeed = 2f; // Saðlýk barýnýn küçülme hýzý
    private float targetHealthRatio = 1f; // Saðlýk oraný
    private bool isMovingDown = false; // Aþaðý hareket kontrolü

    public float downSpeed = 2f; // Aþaðý hareket hýzý
    public float destroyDelay = 4f; // Bot yok olma gecikmesi

    private void Start()
    {
        currentHealth = maxHealth; // Baþlangýçta maksimum can
        UpdateHealthBar(); // Saðlýk barýný baþlat
    }

    private void Update()
    {
        // Sadece saðlýk barýný kameraya doðru döndür
        if (healthBarTransform != null)
        {
            healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - Camera.main.transform.position);
        }

        // Saðlýk barýný yumuþakça güncelle
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, targetHealthRatio, reduceSpeed * Time.deltaTime);

        // Bot aþaðý hareket etmeye baþlamýþsa hareket ettir
        if (isMovingDown)
        {
            transform.Translate(Vector3.down * downSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(1f); // Hasar uygula
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage; // Mevcut caný azalt
        targetHealthRatio = currentHealth / maxHealth; // Saðlýk oranýný hesapla
        UpdateHealthBar(); // Saðlýk barýný güncelle

        // Eðer can sýfýr veya altýna düþerse, yok et
        if (currentHealth <= 0)
        {
            isMovingDown = true; // Bot aþaðý hareket etmeye baþlasýn
            Invoke(nameof(DestroyBot), destroyDelay); // Belirli bir süre sonra yok et
        }
    }

    private void UpdateHealthBar()
    {
        // Saðlýk barýnýn oranýný güncelle
        targetHealthRatio = currentHealth / maxHealth;
    }

    private void DestroyBot()
    {
        Destroy(gameObject); // Botu yok et
    }
}
