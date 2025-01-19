using UnityEngine;
using UnityEngine.UI;

public class BotVuruldu : MonoBehaviour
{
     [SerializeField] private float maxHealth = 3f; // Maksimum can
    private float currentHealth; // Mevcut can

    [SerializeField] private Image healthBarSprite; // Sağlık barının görseli
    [SerializeField] private Transform healthBarTransform; // Sağlık barının transform'u
    [SerializeField] private float healthBarLerpSpeed = 2f; // Sağlık barının geçiş hızı
    private float targetHealthRatio = 1f; // Hedef sağlık oranı
    private bool isMovingDown = false; // Botun aşağı hareket edip etmediği

    public float downSpeed = 2f; // Aşağı hareket hızı
    public float destroyDelay = 2f; // Botun yok olma gecikmesi

    private void Start()
    {
        currentHealth = maxHealth; // Başlangıç canı ayarla
        UpdateHealthBarInstant(); // Sağlık barını başlat
    }

    private void Update()
    {
        // Sağlık barını kameraya döndür
        if (healthBarTransform != null)
        {
            healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - Camera.main.transform.position);
        }

        // Sağlık barının doluluk oranını yumuşakça güncelle
        if (healthBarSprite != null)
        {
            healthBarSprite.fillAmount = Mathf.Lerp(healthBarSprite.fillAmount, targetHealthRatio, healthBarLerpSpeed * Time.deltaTime);
        }

        // Eğer bot aşağı hareket etmeye başladıysa, hareket ettir
        if (isMovingDown)
        {
            transform.Translate(Vector3.down * downSpeed * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Canı azalt
        targetHealthRatio = Mathf.Clamp01(currentHealth / maxHealth); // Sağlık oranını hesapla
        UpdateHealthBarSmooth(); // Sağlık barını güncelle

        // Eğer can sıfır veya daha düşükse, botu yok et
        if (currentHealth <= 0)
        {
            StartDestructionSequence();
        }
    }

    private void UpdateHealthBarInstant()
    {
        // Sağlık barını anında güncelle
        if (healthBarSprite != null)
        {
            healthBarSprite.fillAmount = targetHealthRatio;
        }
    }

    private void UpdateHealthBarSmooth()
    {
        // Sağlık barını yumuşakça güncelle
        if (healthBarSprite != null)
        {
            targetHealthRatio = Mathf.Clamp01(currentHealth / maxHealth);
        }
    }

    private void StartDestructionSequence()
    {
        isMovingDown = true; // Bot aşağı hareket etmeye başlasın
        Invoke(nameof(DestroyBot), destroyDelay); // Yok etme işlemini gecikmeli başlat
    }

    private void DestroyBot()
    {
        Destroy(gameObject); // Botu yok et
    }
}
