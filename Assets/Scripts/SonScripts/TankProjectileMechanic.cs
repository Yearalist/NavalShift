using System;
using UnityEngine;

public class TankProjectileMechanic : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float firePower = 10f;
    public LineRenderer trajectoryLine;
    public int trajectoryResolution = 30;
    public GameObject targetIndicatorPrefab; // Siyah daire için prefab

    private GameObject targetIndicator; // Çizginin bittiği noktayı gösterecek nesne

    private void Start()
    {
        // Hedef göstergesi oluştur
        if (targetIndicatorPrefab != null)
        {
            targetIndicator = Instantiate(targetIndicatorPrefab, Vector3.zero, Quaternion.identity);
            targetIndicator.SetActive(false); // Başta gizli
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
        }

        DrawTrajectory();
    }

    void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile prefab or fire point is not set!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = firePoint.forward * firePower; // Mermiyi fırlat
        }
    }

    public void DrawTrajectory()
    {
        if (trajectoryLine == null || firePoint == null)
            return;

        trajectoryLine.positionCount = trajectoryResolution;
        Vector3[] points = new Vector3[trajectoryResolution];

        Vector3 startPosition = firePoint.position;
        Vector3 startVelocity = firePoint.forward * firePower;

        bool targetFound = false; // Hedef noktayı bulduk mu?
        Vector3 targetPosition = Vector3.zero;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float time = (i / (float)(trajectoryResolution - 1)) * (firePower / Physics.gravity.magnitude);
            points[i] = CalculateTrajectoryPoint(startPosition, startVelocity, time);

            // Eğer y = 0.1 pozisyonuna ulaştıysak hedef noktayı belirle
            if (!targetFound && points[i].y <= 0.1f)
            {
                targetPosition = points[i];
                targetPosition.y = 0.1f; // Y koordinatını sabitliyoruz
                targetFound = true;
            }
        }

        trajectoryLine.SetPositions(points);

        // Hedef göstergesini güncelle
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(targetFound);
            if (targetFound)
            {
                targetIndicator.transform.position = targetPosition;
            }
        }
    }

    Vector3 CalculateTrajectoryPoint(Vector3 startPosition, Vector3 startVelocity, float time)
    {
        return startPosition + startVelocity * time + 0.5f * Physics.gravity * time * time;
    }

    private void OnDrawGizmos()
    {
        // Y ekseni 0.1'de bir daire çiz
        if (targetIndicator != null && targetIndicator.activeSelf)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(targetIndicator.transform.position, 0.2f); // Daire boyutu
        }
    }
}