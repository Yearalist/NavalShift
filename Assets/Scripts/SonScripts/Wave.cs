using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public int Dimension = 10; // Izgara boyutu, yüzeyin geniþliði ve yüksekliði (10x10 birimlik bir yüzey olacak).


    public float UVScale = 2f;

    public Octave[] Octaves; // Dalga özelliklerini tutan yapý (örneðin hýz, yükseklik, ölçek).

    protected MeshFilter MeshFilter; // MeshFilter bileþeni, mesh'i sahnede göstermek için kullanýlýr.
    protected Mesh Mesh; // Yüzeyi tanýmlayan mesh (köþe noktalarý ve üçgenlerden oluþur).

    void Start()
    {
        // Mesh Setup: Yeni bir mesh oluþtur ve bu mesh'i yapýlandýr.
        Mesh = new Mesh();
        Mesh.name = gameObject.name; // Mesh'e bu nesnenin adýný ver.

        // Mesh'in köþe noktalarýný ve üçgenlerini oluþtur.
        Mesh.vertices = GenerateVerts(); // Köþe noktalarýný hesapla.
        Mesh.triangles = GenerateTries(); // Üçgenleri hesapla.

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds(); // Mesh'in sýnýrlarýný yeniden hesapla (görünürlük için gerekli).

        // Bu nesneye bir MeshFilter bileþeni ekle ve oluþturduðun mesh'i ata.
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
    }


    public float GetHeight(Vector3 position)
    {
        // Ölçek faktörü ve yerel uzaydaki pozisyonu hesapla
        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale((position - transform.position), scale);

        // Kenar noktalarýný al
        var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        // Pozisyon düzlemin dýþýndaysa sýnýrlama yap
        p1.x = Mathf.Clamp(p1.x, 0, Dimension);
        p1.z = Mathf.Clamp(p1.z, 0, Dimension);
        p2.x = Mathf.Clamp(p2.x, 0, Dimension);
        p2.z = Mathf.Clamp(p2.z, 0, Dimension);
        p3.x = Mathf.Clamp(p3.x, 0, Dimension);
        p3.z = Mathf.Clamp(p3.z, 0, Dimension);
        p4.x = Mathf.Clamp(p4.x, 0, Dimension);
        p4.z = Mathf.Clamp(p4.z, 0, Dimension);

        // Kenarlara olan maksimum mesafeyi al ve aðýrlýklý hesapla
        var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        var dist = (max - Vector3.Distance(p1, localPos))
                 + (max - Vector3.Distance(p2, localPos))
                 + (max - Vector3.Distance(p3, localPos))
                 + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        // Aðýrlýklý toplam
        var height = Mesh.vertices[index(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                   + Mesh.vertices[index(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                   + Mesh.vertices[index(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                   + Mesh.vertices[index(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));

        // Yüksekliði ölçekle
        return height * transform.lossyScale.y / dist;

    }

    private Vector3[] GenerateVerts()
    {
        // Tüm vertexleri oluþtur
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        // Eþit aralýklarla vertexleri daðýt
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
                verts[index(x, z)] = new Vector3(x, 0, z);

        return verts;
    }

    private int[] GenerateTries()
    {
        // Üçgen dizilerini oluþtur
        var tries = new int[Mesh.vertices.Length * 6];

        // Ýki üçgen bir kare oluþturur
        for (int x = 0; x < Dimension; x++)
        {
            for (int z = 0; z < Dimension; z++)
            {
                tries[index(x, z) * 6 + 0] = index(x, z);
                tries[index(x, z) * 6 + 1] = index(x + 1, z + 1);
                tries[index(x, z) * 6 + 2] = index(x + 1, z);
                tries[index(x, z) * 6 + 3] = index(x, z);
                tries[index(x, z) * 6 + 4] = index(x, z + 1);
                tries[index(x, z) * 6 + 5] = index(x + 1, z + 1);
            }
        }

        return tries;
    }

    private Vector2[] GenerateUVs()
    {
        // UV koordinatlarýný oluþtur
        var uvs = new Vector2[Mesh.vertices.Length];

        // UV koordinatlarýný eþit aralýklarla ayarla
        for (int x = 0; x <= Dimension; x++)
        {
            for (int z = 0; z <= Dimension; z++)
            {
                var vec = new Vector2((x / UVScale) % 2, (z / UVScale) % 2);
                uvs[index(x, z)] = new Vector2(vec.x <= 1 ? vec.x : 2 - vec.x, vec.y <= 1 ? vec.y : 2 - vec.y);
            }
        }

        return uvs;
    }

    private int index(int x, int z)
    {
        // Verilen x ve z koordinatlarý için vertex dizini hesapla
        return x * (Dimension + 1) + z;
    }

    private int index(float x, float z)
    {
        // float türündeki x ve z deðerlerini tamsayýya çevirerek index fonksiyonunu çaðýr
        return index((int)x, (int)z);
    }

    // Update is called once per frame
    void Update()
    {
        // Mesh'in vertexlerini güncelle

        var verts = Mesh.vertices;
        for (int x = 0; x <= Dimension; x++)
        {
            for (int z = 0; z <= Dimension; z++)
            {
                var y = 0f;
                for (int o = 0; o < Octaves.Length; o++)
                {
                    if (Octaves[o].alternate)
                    {
                        // Alternatif gürültü hesaplama
                        var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x) / Dimension, (z * Octaves[o].scale.y) / Dimension) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + Octaves[o].speed.magnitude * Time.time) * Octaves[o].height;
                    }
                    else
                    {
                        // Standart Perlin gürültüsü hesaplama
                        var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x + Time.time * Octaves[o].speed.x) / Dimension, (z * Octaves[o].scale.y + Time.time * Octaves[o].speed.y) / Dimension) - 0.5f;
                        y += perl * Octaves[o].height;
                    }
                }
                // Vertex'in yüksekliðini güncelle

                verts[index(x, z)] = new Vector3(x, y, z);
            }
        }
        Mesh.vertices = verts;
        Mesh.RecalculateNormals();// Normalleri yeniden hesapla
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed; // Gürültünün hareket hýzý
        public Vector2 scale; // Gürültünün ölçek faktörü
        public float height; // Yükseklik katkýsý
        public bool alternate; // Alternatif gürültü kullanýmý
    }
}