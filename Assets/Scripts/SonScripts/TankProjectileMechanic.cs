using System;
using Unity.VisualScripting;
using UnityEngine;

public class TankProjectileMechanic : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float firePower = 10f;
    public LineRenderer trajectoryLine;
    public int trajectoryResolution = 30;

    private void Update()
    {
        DrawTrajectory();

        if (Input.GetButtonDown("Fire1")) 
        {
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * firePower; 
            }
        }
    }

    public void DrawTrajectory()
    {
        if (trajectoryLine != null && firePoint != null)
        {
            //trajectoryResolution = Mathf.Clamp((int)(firePower * 2), 10, 100);

            trajectoryLine.positionCount = trajectoryResolution;
            Vector3[] points = new Vector3[trajectoryResolution];

            // Her seferinde en güncel pozisyonu ve yönü al
            Vector3 startPosition = firePoint.position;
            Vector3 startVelocity = firePoint.forward * firePower;

            for (int i = 0; i < trajectoryResolution; i++)
            {
                float time = i / (float)trajectoryResolution;
                points[i] = CalculateTrajectoryPoint(startPosition, startVelocity, time);
            }

            trajectoryLine.SetPositions(points);
        }
    }

    Vector3 CalculateTrajectoryPoint(Vector3 startPosition, Vector3 startVelocity, float time)
    {
        // Fizik Hareket denklemi: s = s0 + v0 * t + 0.5 * g * t^2
        Vector3 gravity = Physics.gravity;
        // İvmeli hareket için yer çekiminin etkisini kullandım
        return startPosition + startVelocity * time + 0.5f * gravity * time * time;
    }
}
