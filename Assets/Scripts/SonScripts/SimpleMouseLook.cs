using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float sensitivity = 2f; // Mouse duyarl�l���
    public Transform tankBody;     // Tank�n g�vdesi
    public Transform tankTurret;   // Tank�n kulesi

    private float verticalRotation = 0f; // Dikey a��
    private float horizontalRotation = 0f; // Yatay a��

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Fareyi kilitle
        Cursor.visible = false;                  // Fare imlecini gizle
    }

    void Update()
    {
        // Mouse hareketini al
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // Yatay d�n�� (g�vde i�in)
        horizontalRotation += mouseX;
        tankBody.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Dikey d�n�� (kule i�in)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -15f, 15f); // Daha dar dikey a��
        tankTurret.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
