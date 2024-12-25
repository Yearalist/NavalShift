using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class ShipmainMovement : MonoBehaviour
{
    
    [Header("Hareket Hızı")]
    [SerializeField] private float horizontalSpeed = 5f; // X ekseni için hız
    [SerializeField] private float verticalSpeed = 5f;   // Z ekseni için hız
     
    [Header("Hareket Limitleri")]
    [SerializeField] private Vector2 horizontalBounds = new Vector2(-5f, 5f); // X ekseni sınırları
    [SerializeField] private Vector2 verticalBounds = new Vector2(-5f, 5f); 

    private bool canMoveHorizontally = false; // Yatay hareket aktif mi?
    private bool canMoveVertically = false;  // Dikey hareket aktif mi?

    void Start()
    {
        // Başlangıç ayarı: Geminin pembe bölgeden başlamasını istiyorsan burada ayar yapabilirsin
        canMoveHorizontally = false;
        canMoveVertically = true;
    }

    void Update()
    {
        MoveShip();
    }

    private void MoveShip()
    {
        Vector3 movement = Vector3.zero;

        // Sadece yatay hareket (X ekseni)
        if (canMoveHorizontally)
        {
            float horizontalInput = Input.GetKey(KeyCode.A) ? -1f : (Input.GetKey(KeyCode.D) ? 1f : 0f);
            movement.x = horizontalInput * horizontalSpeed * Time.deltaTime;
        }

        // Sadece dikey hareket (Z ekseni)
        if (canMoveVertically)
        {
            float verticalInput = Input.GetKey(KeyCode.W) ? 1f : (Input.GetKey(KeyCode.S) ? -1f : 0f);
            movement.z = verticalInput * verticalSpeed * Time.deltaTime;
        }

        // Hareket uygula
        transform.position += movement;


        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, horizontalBounds.x, horizontalBounds.y),
            transform.position.y,
            Mathf.Clamp(transform.position.z, verticalBounds.x, verticalBounds.y));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PinkZone")) // Pembe bölgeye girerse
        {
            canMoveHorizontally = false; // Yatay hareket devre dışı
            canMoveVertically = true;   // Dikey hareket aktif
        }
        else if (other.CompareTag("WhiteZone")) // Beyaz bölgeye girerse
        {
            canMoveHorizontally = true; // Yatay hareket aktif
            canMoveVertically = false; // Dikey hareket devre dışı
        }
    }
    }

