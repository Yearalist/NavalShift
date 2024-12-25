using Ditzelgames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloat : MonoBehaviour
{
    public float AirDrag = 1; // Hava sürtünmesi
    public float WaterDrag = 10; // Su sürtünmesi
    public bool AffectDirection = true; // Yönlendirme etkilenmeli mi?
    public bool AttachToSurface = false; // Yüzeyde sabitlenmeli mi?
    public Transform[] FloatPoints; // Yüzen noktalar (nesnenin suya temas eden noktalarý)

    // Kullanýlan bileþenler
    protected Rigidbody Rigidbody; // Nesneye baðlý Rigidbody bileþeni
    protected Wave Waves; // Dalga bileþeni (su yüzeyi hareketleri)

    // Su çizgisi
    protected float WaterLine; // Su çizgisi yüksekliði
    protected Vector3[] WaterLinePoints; // Su çizgisi noktalarý

    // Yardýmcý vektörler
    protected Vector3 smoothVectorRotation; // Yumuþak dönüþ vektörü
    protected Vector3 TargetUp; // Hedef yukarý vektörü
    protected Vector3 centerOffset; // Merkez kaymasý

    // Nesnenin merkezi
    public Vector3 Center { get { return transform.position + centerOffset; } }

    // Baþlangýçta bir kez çalýþýr
    void Awake()
    {
        // Bileþenleri al
        Waves = FindObjectOfType<Wave>(); // Sahnedeki Wave bileþenini bul
        Rigidbody = GetComponent<Rigidbody>(); // Rigidbody bileþenini al
        Rigidbody.useGravity = false; // Yerçekimini devre dýþý býrak

        // Merkezi hesapla
        WaterLinePoints = new Vector3[FloatPoints.Length]; // Yüzme noktalarýný baþlat
        for (int i = 0; i < FloatPoints.Length; i++)
            WaterLinePoints[i] = FloatPoints[i].position;
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position; // Merkez kaymasýný hesapla
    }

    // Her fizik güncellemesinde çalýþýr
    void FixedUpdate()
    {
        // Varsayýlan su yüzeyi yüksekliði
        var newWaterLine = 0f;
        var pointUnderWater = false; // Su altýnda olan noktalar kontrolü

        // Su çizgisi noktalarýný ve su çizgisini ayarla
        for (int i = 0; i < FloatPoints.Length; i++)
        {
            // Yüksekliði hesapla
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position); // Su yüzeyindeki yüksekliði al
            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length; // Ortalama su çizgisi yüksekliði
            if (WaterLinePoints[i].y > FloatPoints[i].position.y) // Su altýnda olup olmadýðýný kontrol et
                pointUnderWater = true;
        }

        var waterLineDelta = newWaterLine - WaterLine; // Su çizgisindeki deðiþim
        WaterLine = newWaterLine; // Yeni su çizgisini güncelle

        // Yukarý vektörünü hesapla
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        // Yerçekimi hesapla
        var gravity = Physics.gravity; // Varsayýlan yerçekimi
        Rigidbody.linearDamping = AirDrag; // Hava sürtünmesi uygula
        if (WaterLine > Center.y) // Eðer nesne suyun altýndaysa
        {
            Rigidbody.linearDamping = WaterDrag; // Su sürtünmesi uygula
            if (AttachToSurface) // Eðer yüzeye sabitlenmeli ise
            {
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity; // Yerçekimini yönlendir
                transform.Translate(Vector3.up * waterLineDelta * 0.9f); // Su çizgisine doðru hareket et
            }
        }
        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1)); // Yerçekimi kuvvetini uygula

        // Dönüþ hesaplamasý
        if (pointUnderWater) // Eðer herhangi bir nokta su altýndaysa
        {
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f); // Yumuþak dönüþ uygula
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation; // Dönüþü uygula
        }
    }

    // Çizim iþlevi (Gizmos)
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
                Gizmos.color = Color.red; // Su çizgisi noktalarýný çiz
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            Gizmos.color = Color.green; // Yüzme noktalarýný çiz
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        // Merkezi çiz
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }
}