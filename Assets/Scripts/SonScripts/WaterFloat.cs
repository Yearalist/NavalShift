using Ditzelgames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloat : MonoBehaviour
{
    public float AirDrag = 1; // Hava s�rt�nmesi
    public float WaterDrag = 10; // Su s�rt�nmesi
    public bool AffectDirection = true; // Y�nlendirme etkilenmeli mi?
    public bool AttachToSurface = false; // Y�zeyde sabitlenmeli mi?
    public Transform[] FloatPoints; // Y�zen noktalar (nesnenin suya temas eden noktalar�)

    // Kullan�lan bile�enler
    protected Rigidbody Rigidbody; // Nesneye ba�l� Rigidbody bile�eni
    protected Wave Waves; // Dalga bile�eni (su y�zeyi hareketleri)

    // Su �izgisi
    protected float WaterLine; // Su �izgisi y�ksekli�i
    protected Vector3[] WaterLinePoints; // Su �izgisi noktalar�

    // Yard�mc� vekt�rler
    protected Vector3 smoothVectorRotation; // Yumu�ak d�n�� vekt�r�
    protected Vector3 TargetUp; // Hedef yukar� vekt�r�
    protected Vector3 centerOffset; // Merkez kaymas�

    // Nesnenin merkezi
    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Ba�lang��ta bir kez �al���r
    void Awake()
    {
        // Bile�enleri al
        Waves = FindObjectOfType<Wave>(); // Sahnedeki Wave bile�enini bul
        Rigidbody = GetComponent<Rigidbody>(); // Rigidbody bile�enini al
        Rigidbody.useGravity = false; // Yer�ekimini devre d��� b�rak

        // Merkezi hesapla
        WaterLinePoints = new Vector3[FloatPoints.Length]; // Y�zme noktalar�n� ba�lat
        for (int i = 0; i < FloatPoints.Length; i++)
            WaterLinePoints[i] = FloatPoints[i].position;
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position; // Merkez kaymas�n� hesapla
    }

    // Her fizik g�ncellemesinde �al���r
    void FixedUpdate()
    {
        // Varsay�lan su y�zeyi y�ksekli�i
        var newWaterLine = 0f;
        var pointUnderWater = false; // Su alt�nda olan noktalar kontrol�

        // Su �izgisi noktalar�n� ve su �izgisini ayarla
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            // Y�ksekli�i hesapla
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position); // Su y�zeyindeki y�ksekli�i al
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length; // Ortalama su �izgisi y�ksekli�i
            if (WaterLinePoints[i].y > FloatPoints[i].position.y) // Su alt�nda olup olmad���n� kontrol et
                pointUnderWater = true;
        }

        var waterLineDelta = newWaterLine - WaterLine; // Su �izgisindeki de�i�im
        WaterLine = newWaterLine; // Yeni su �izgisini g�ncelle

        // Yukar� vekt�r�n� hesapla
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        // Yer�ekimi hesapla
        var gravity = Physics.gravity; // Varsay�lan yer�ekimi
        Rigidbody.linearDamping = AirDrag; // Hava s�rt�nmesi uygula
        if (WaterLine > Center.y) // E�er nesne suyun alt�ndaysa
        {
            Rigidbody.linearDamping = WaterDrag; // Su s�rt�nmesi uygula
            if (AttachToSurface) // E�er y�zeye sabitlenmeli ise
            {
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity; // Yer�ekimini y�nlendir
                transform.Translate(Vector3.up * waterLineDelta * 0.9f); // Su �izgisine do�ru hareket et
            }
        }
        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1)); // Yer�ekimi kuvvetini uygula

        // D�n�� hesaplamas�
        if (pointUnderWater) // E�er herhangi bir nokta su alt�ndaysa
        {
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f); // Yumu�ak d�n�� uygula
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation; // D�n��� uygula
        }
    }

    // �izim i�levi (Gizmos)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (FloatPoints == null)
            return;

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            if (FloatPoints[i] == null)
                continue;

            if (Waves != null)
            {
                Gizmos.color = Color.red; // Su �izgisi noktalar�n� �iz
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            Gizmos.color = Color.green; // Y�zme noktalar�n� �iz
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        // Merkezi �iz
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }
}