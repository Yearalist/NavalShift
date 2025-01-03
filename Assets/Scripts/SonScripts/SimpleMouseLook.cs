using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float sensitivity = 2f; // Mouse duyarlýlýðý
    public Transform tankBody;     // Tankýn gövdesi
    public Transform tankTurret;   // Tankýn kulesi

    private float verticalRotation = 0f; // Dikey açý
    private float horizontalRotation = 0f; // Yatay açý

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

        // Yatay dönüþ (gövde için)
        horizontalRotation += mouseX;
        tankBody.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Dikey dönüþ (kule için)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -15f, 15f); // Daha dar dikey açý
        tankTurret.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
