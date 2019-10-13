using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CasualGodComplex;
using CasualGodComplex.Galaxies;

public class Galaxy : MonoBehaviour
{
    public Texture2D galaxyGrayscale;
    public Material starMaterial;
    public int Seed;

    [HideInInspector]
    public float Distance = 0.1f;
    [HideInInspector]
    public float Scale = 0.001f;

    private float textureWidth;
    private float textureHeight;
    private float sectorSize = 750;
    private SphereGalaxy sphereGalaxy;
    private const float countMean = 0.0000025f;

    void Start()
    {
        textureWidth = galaxyGrayscale.width;
        textureHeight = galaxyGrayscale.height;
        int count = 0;

        Scale = transform.localScale.x / 10000;
        Distance = transform.localScale.x;

        for (int y = 0; y < textureHeight; y += 16)
        {
            for (int x = 0; x < textureWidth; x += 16)
            {
                Color c = galaxyGrayscale.GetPixel(x, y);
                if (c.grayscale > 0.1f)
                {
                    sphereGalaxy = new SphereGalaxy(sectorSize, countMean * (c.grayscale / 5));
                    ClusterGalaxy clusterGalaxy = new ClusterGalaxy(sphereGalaxy);
                    GameObject obj = new GameObject("Sector: " + x + ", " + y);
                    obj.transform.parent = transform;
                    obj.transform.position = transform.localPosition + new Vector3(x * transform.localScale.x, transform.localPosition.y, y * transform.localScale.y);
                    obj.AddComponent<Sector>().Cluster = clusterGalaxy;
                    obj.GetComponent<Sector>().Galaxy = this;
                    obj.GetComponent<Sector>().Seed = Seed + count;
                }
                count++;
            }
        }
    }
}
