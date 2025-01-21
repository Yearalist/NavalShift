using UnityEngine;
using UnityEngine.UI;

public class KuleVuruldu : MonoBehaviour
{
    [SerializeField] private float maxHealth = 500f; // Maksimum can
    private float currentHealth; // Mevcut can

    [SerializeField] private Image healthBarSprite; // Sağlık barının görseli
    [SerializeField] private Transform healthBarTransform; // Sağlık barının transform'u
    [SerializeField] private float healthBarLerpSpeed = 2f; // Sağlık barının geçiş hızı
    private float targetHealthRatio = 1f; // Hedef sağlık oranı

    public float destroyDelay = 2f; // Kule yok olma gecikmesi

    private void Start()
    {
        currentHealth = maxHealth; // Başlangıç canını maksimum cana eşitle
        targetHealthRatio = 1f; // Hedef sağlık oranını %100 olarak başlat
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
    }

    public void TakeDamage(float damage)
    {
        Debug.Log($"Kule hasar aldı: {damage}");
        currentHealth -= damage; // Canı azalt
        targetHealthRatio = Mathf.Clamp01(currentHealth / maxHealth); // Sağlık oranını hesapla
        UpdateHealthBarSmooth(); // Sağlık barını güncelle

        if (currentHealth <= 0)
        {
            Debug.Log("Kule yok ediliyor...");
            StartDestructionSequence();
        }
    }

    private void UpdateHealthBarInstant()
    {
        // Sağlık barını anında güncelle
        if (healthBarSprite != null)
        {
            healthBarSprite.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
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
        // Kuleyi yok etme işlemini başlat
        Invoke(nameof(DestroyKule), destroyDelay);
    }

    private void DestroyKule()
    {
        Destroy(gameObject); // Kuleyi yok et
    }
}
