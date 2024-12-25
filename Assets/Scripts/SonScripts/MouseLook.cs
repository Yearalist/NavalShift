
using UnityEngine;


public class MouseLook : MonoBehaviour
    {
    
        public float sensitivity = 5f;
        public float minVerticalAngle = -25f;
        public float maxVerticalAngle = 25f;

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
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // Tank gövdesini yatay döndür
            rotationY += mouseX * sensitivity;
            tankBody.rotation = Quaternion.Euler(0f, rotationY, 0f);

            // Kuleyi dikey döndür
            rotationX -= mouseY * sensitivity;
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
            tankTurret.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

            // Trajektory çizgiyi güncelle
            tankProjectileMechanic.DrawTrajectory(); // Burada sürekli çağırıyoruz
        }
    }
