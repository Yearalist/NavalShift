using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensitivity = 3f; // Duyarlılığı biraz düşürdük
    public float minVerticalAngle = -20f; // Daha dar bir dikey açı
    public float maxVerticalAngle = 20f;

    public Transform tankBody;
    public Transform tankTurret;

    private float rotationX = 0f;
    private float rotationY = 0f;

    public TankProjectileMechanic tankProjectileMechanic;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Mouse hareketini al
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Gövdeyi sadece yatay döndür
        rotationY += mouseX;
        tankBody.rotation = Quaternion.Euler(0f, rotationY, 0f);

        // Kuleyi dikey döndür
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
        tankTurret.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Trajektory çizgiyi gerektiğinde güncelle
        if (Input.GetMouseButton(0)) // Sadece sol tıklama sırasında çizgi güncellenir
        {
            tankProjectileMechanic.DrawTrajectory();
        }
    }
}
