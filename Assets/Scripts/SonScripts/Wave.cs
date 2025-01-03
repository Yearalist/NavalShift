using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public int Dimension = 10; // Izgara boyutu, y�zeyin geni�li�i ve y�ksekli�i (10x10 birimlik bir y�zey olacak).


    public float UVScale = 2f;

    public Octave[] Octaves; // Dalga �zelliklerini tutan yap� (�rne�in h�z, y�kseklik, �l�ek).

    protected MeshFilter MeshFilter; // MeshFilter bile�eni, mesh'i sahnede g�stermek i�in kullan�l�r.
    protected Mesh Mesh; // Y�zeyi tan�mlayan mesh (k��e noktalar� ve ��genlerden olu�ur).

    void Start()
    {
        // Mesh Setup: Yeni bir mesh olu�tur ve bu mesh'i yap�land�r.
        Mesh = new Mesh();
        Mesh.name = gameObject.name; // Mesh'e bu nesnenin ad�n� ver.

        // Mesh'in k��e noktalar�n� ve ��genlerini olu�tur.
        Mesh.vertices = GenerateVerts(); // K��e noktalar�n� hesapla.
        Mesh.triangles = GenerateTries(); // ��genleri hesapla.

        Mesh.RecalculateNormals();
        Mesh.RecalculateBounds(); // Mesh'in s�n�rlar�n� yeniden hesapla (g�r�n�rl�k i�in gerekli).

        // Bu nesneye bir MeshFilter bile�eni ekle ve olu�turdu�un mesh'i ata.
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshFilter.mesh = Mesh;
    }


    public float GetHeight(Vector3 position)
    {
        // �l�ek fakt�r� ve yerel uzaydaki pozisyonu hesapla
        var scale = new Vector3(1 / transform.lossyScale.x, 0, 1 / transform.lossyScale.z);
        var localPos = Vector3.Scale((position - transform.position), scale);

        // Kenar noktalar�n� al
        var p1 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Floor(localPos.z));
        var p2 = new Vector3(Mathf.Floor(localPos.x), 0, Mathf.Ceil(localPos.z));
        var p3 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Floor(localPos.z));
        var p4 = new Vector3(Mathf.Ceil(localPos.x), 0, Mathf.Ceil(localPos.z));

        // Pozisyon d�zlemin d���ndaysa s�n�rlama yap
        p1.x = Mathf.Clamp(p1.x, 0, Dimension);
        p1.z = Mathf.Clamp(p1.z, 0, Dimension);
        p2.x = Mathf.Clamp(p2.x, 0, Dimension);
        p2.z = Mathf.Clamp(p2.z, 0, Dimension);
        p3.x = Mathf.Clamp(p3.x, 0, Dimension);
        p3.z = Mathf.Clamp(p3.z, 0, Dimension);
        p4.x = Mathf.Clamp(p4.x, 0, Dimension);
        p4.z = Mathf.Clamp(p4.z, 0, Dimension);

        // Kenarlara olan maksimum mesafeyi al ve a��rl�kl� hesapla
        var max = Mathf.Max(Vector3.Distance(p1, localPos), Vector3.Distance(p2, localPos), Vector3.Distance(p3, localPos), Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        var dist = (max - Vector3.Distance(p1, localPos))
                 + (max - Vector3.Distance(p2, localPos))
                 + (max - Vector3.Distance(p3, localPos))
                 + (max - Vector3.Distance(p4, localPos) + Mathf.Epsilon);
        // A��rl�kl� toplam
        var height = Mesh.vertices[index(p1.x, p1.z)].y * (max - Vector3.Distance(p1, localPos))
                   + Mesh.vertices[index(p2.x, p2.z)].y * (max - Vector3.Distance(p2, localPos))
                   + Mesh.vertices[index(p3.x, p3.z)].y * (max - Vector3.Distance(p3, localPos))
                   + Mesh.vertices[index(p4.x, p4.z)].y * (max - Vector3.Distance(p4, localPos));

        // Y�ksekli�i �l�ekle
        return height * transform.lossyScale.y / dist;

    }

    private Vector3[] GenerateVerts()
    {
        // T�m vertexleri olu�tur
        var verts = new Vector3[(Dimension + 1) * (Dimension + 1)];

        // E�it aral�klarla vertexleri da��t
        for (int x = 0; x <= Dimension; x++)
            for (int z = 0; z <= Dimension; z++)
                verts[index(x, z)] = new Vector3(x, 0, z);

        return verts;
    }

    private int[] GenerateTries()
    {
        // ��gen dizilerini olu�tur
        var tries = new int[Mesh.vertices.Length * 6];

        // �ki ��gen bir kare olu�turur
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
        // UV koordinatlar�n� olu�tur
        var uvs = new Vector2[Mesh.vertices.Length];

        // UV koordinatlar�n� e�it aral�klarla ayarla
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
        // Verilen x ve z koordinatlar� i�in vertex dizini hesapla
        return x * (Dimension + 1) + z;
    }

    private int index(float x, float z)
    {
        // float t�r�ndeki x ve z de�erlerini tamsay�ya �evirerek index fonksiyonunu �a��r
        return index((int)x, (int)z);
    }

    // Update is called once per frame
    void Update()
    {
        // Mesh'in vertexlerini g�ncelle

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
                        // Alternatif g�r�lt� hesaplama
                        var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x) / Dimension, (z * Octaves[o].scale.y) / Dimension) * Mathf.PI * 2f;
                        y += Mathf.Cos(perl + Octaves[o].speed.magnitude * Time.time) * Octaves[o].height;
                    }
                    else
                    {
                        // Standart Perlin g�r�lt�s� hesaplama
                        var perl = Mathf.PerlinNoise((x * Octaves[o].scale.x + Time.time * Octaves[o].speed.x) / Dimension, (z * Octaves[o].scale.y + Time.time * Octaves[o].speed.y) / Dimension) - 0.5f;
                        y += perl * Octaves[o].height;
                    }
                }
                // Vertex'in y�ksekli�ini g�ncelle

                verts[index(x, z)] = new Vector3(x, y, z);
            }
        }
        Mesh.vertices = verts;
        Mesh.RecalculateNormals();// Normalleri yeniden hesapla
    }

    [Serializable]
    public struct Octave
    {
        public Vector2 speed; // G�r�lt�n�n hareket h�z�
        public Vector2 scale; // G�r�lt�n�n �l�ek fakt�r�
        public float height; // Y�kseklik katk�s�
        public bool alternate; // Alternatif g�r�lt� kullan�m�
    }
}