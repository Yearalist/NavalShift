using UnityEngine;
using UnityEngine.UI;

public class BotVuruldu : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f; // Maksimum can
    private float currentHealth; // Mevcut can

    [SerializeField] private Image healthBarSprite; // Sa�l�k bar� g�rseli
    [SerializeField] private Transform healthBarTransform; // Sa�l�k bar�n�n transformu
    [SerializeField] private float reduceSpeed = 2f; // Sa�l�k bar�n�n k���lme h�z�
    private float targetHealthRatio = 1f; // Sa�l�k oran�
    private bool isMovingDown = false; // A�a�� hareket kontrol�

    public float downSpeed = 2f; // A�a�� hareket h�z�
    public float destroyDelay = 4f; // Bot yok olma gecikmesi

    private void Start()
    {
        currentHealth = maxHealth; // Ba�lang��ta maksimum can
        UpdateHealthBar(); // Sa�l�k bar�n� ba�lat
    }

    private void Update()
    {
        // Sadece sa�l�k bar�n� kameraya do�ru d�nd�r
        if (healthBarTransform != null)
        {
            healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - Camera.main.transform.position);
        }

        // Sa�l�k bar�n� yumu�ak�a g�ncelle
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, targetHealthRatio, reduceSpeed * Time.deltaTime);

        // Bot a�a�� hareket etmeye ba�lam��sa hareket ettir
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
        currentHealth -= damage; // Mevcut can� azalt
        targetHealthRatio = currentHealth / maxHealth; // Sa�l�k oran�n� hesapla
        UpdateHealthBar(); // Sa�l�k bar�n� g�ncelle

        // E�er can s�f�r veya alt�na d��erse, yok et
        if (currentHealth <= 0)
        {
            isMovingDown = true; // Bot a�a�� hareket etmeye ba�las�n
            Invoke(nameof(DestroyBot), destroyDelay); // Belirli bir s�re sonra yok et
        }
    }

    private void UpdateHealthBar()
    {
        // Sa�l�k bar�n�n oran�n� g�ncelle
        targetHealthRatio = currentHealth / maxHealth;
    }

    private void DestroyBot()
    {
        Destroy(gameObject); // Botu yok et
    }
}
