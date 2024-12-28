using Ditzelgames;
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterFloat : MonoBehaviour
{
    public float AirDrag = 1;
    public float WaterDrag = 10;
    public bool AffectDirection = true;
    public bool AttachToSurface = false;
    public Transform[] FloatPoints;

    protected Rigidbody Rigidbody;
    protected Wave Waves;

    protected float WaterLine;
    protected Vector3[] WaterLinePoints;

    protected Vector3 smoothVectorRotation;
    protected Vector3 TargetUp;
    protected Vector3 centerOffset;

    private float updateInterval = 0.1f; // Yüzme noktasý güncelleme aralýðý
    private float lastUpdateTime = 0f;

    public Vector3 Center { get { return transform.position + centerOffset; } }

    void Awake()
    {
        Waves = FindObjectOfType<Wave>();
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.useGravity = false;

        WaterLinePoints = new Vector3[FloatPoints.Length];
        for (int i = 0; i < FloatPoints.Length; i++)
            WaterLinePoints[i] = FloatPoints[i].position;
        centerOffset = PhysicsHelper.GetCenter(WaterLinePoints) - transform.position;
    }

    void FixedUpdate()
    {
        if (Time.time - lastUpdateTime >= updateInterval) // Belirli aralýklarla güncelle
        {
            UpdateWaterLinePoints();
            lastUpdateTime = Time.time;
        }

        UpdatePhysics();
    }

    private void UpdateWaterLinePoints()
    {
        var newWaterLine = 0f;
        var pointUnderWater = false;

        for (int i = 0; i < FloatPoints.Length; i++)
        {
            WaterLinePoints[i] = FloatPoints[i].position;
            WaterLinePoints[i].y = Waves.GetHeight(FloatPoints[i].position); // Optimize edilmiþ dalga yüksekliði

            newWaterLine += WaterLinePoints[i].y / FloatPoints.Length;
            if (WaterLinePoints[i].y > FloatPoints[i].position.y)
                pointUnderWater = true;
        }

        WaterLine = newWaterLine;
        TargetUp = PhysicsHelper.GetNormal(WaterLinePoints);

        if (pointUnderWater)
        {
            TargetUp = Vector3.SmoothDamp(transform.up, TargetUp, ref smoothVectorRotation, 0.2f);
            Rigidbody.rotation = Quaternion.FromToRotation(transform.up, TargetUp) * Rigidbody.rotation;
        }
    }

    private void UpdatePhysics()
    {
        var gravity = Physics.gravity;
        Rigidbody.drag = AirDrag;

        if (WaterLine > Center.y)
        {
            Rigidbody.drag = WaterDrag;

            if (AttachToSurface)
            {
                Rigidbody.position = new Vector3(Rigidbody.position.x, WaterLine - centerOffset.y, Rigidbody.position.z);
            }
            else
            {
                gravity = AffectDirection ? TargetUp * -Physics.gravity.y : -Physics.gravity;
                transform.Translate(Vector3.up * (WaterLine - Center.y) * 0.9f);
            }
        }

        Rigidbody.AddForce(gravity * Mathf.Clamp(Mathf.Abs(WaterLine - Center.y), 0, 1));
    }

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
                Gizmos.color = Color.red;
                Gizmos.DrawCube(WaterLinePoints[i], Vector3.one * 0.3f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(FloatPoints[i].position, 0.1f);
        }

        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(new Vector3(Center.x, WaterLine, Center.z), Vector3.one * 1f);
            Gizmos.DrawRay(new Vector3(Center.x, WaterLine, Center.z), TargetUp * 1f);
        }
    }
}