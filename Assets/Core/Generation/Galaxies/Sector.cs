using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CasualGodComplex;
using CasualGodComplex.Galaxies;

public class Sector : MonoBehaviour
{
    public ClusterGalaxy Cluster;
    public Galaxy Galaxy;
    public int Seed = 1;

    private Transform _cam;
    private List<GameObject> stars = new List<GameObject>();
    private float generationDistance = 10000f;
    private bool generated = false;

    private void Start()
    {
        generationDistance = Galaxy.transform.localScale.x * 100;
        _cam = Camera.main.transform;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, _cam.position) < generationDistance && !generated)
        {
            generated = true;
            GenerateStars();
        }
        else if(Vector3.Distance(transform.position, _cam.position) > generationDistance && generated)
        {
            generated = false;
            DestroyStars();
        }
    }

    private void DestroyStars()
    {
        for(int i = 0; i < stars.Count; i++)
        {
            Destroy(stars[i]);
        }
    }

    private void GenerateStars()
    {
        IEnumerable<Star> starsEnum = Cluster.Generate(new System.Random(Seed));
        int starSeed = Seed;

        foreach (Star star in starsEnum)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(obj.GetComponent<BoxCollider>());
            obj.name = "Star: " + star.Name;
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3(star.Position.X * Galaxy.Distance * 10000, star.Position.Y * Galaxy.Distance * 10000, star.Position.Z * Galaxy.Distance * 10000);
            float sizeObj = star.Size * Galaxy.Scale;
            obj.transform.localScale = new Vector3(sizeObj, sizeObj, sizeObj);
            Color color = Mathf.CorrelatedColorTemperatureToRGB(star.Temperature);
            obj.GetComponent<MeshRenderer>().material = Galaxy.starMaterial;
            obj.GetComponent<MeshRenderer>().material.SetColor("_TintColor", color);
            obj.GetComponent<MeshRenderer>().material.SetFloat("_Scale", obj.transform.localScale.x);
            obj.AddComponent<StarBehaviour>().Scale = sizeObj * 10;
            obj.GetComponent<StarBehaviour>().Temperature = star.Temperature;
            obj.GetComponent<StarBehaviour>().Name = star.Name;
            obj.GetComponent<StarBehaviour>().Mass = star.Mass;
            obj.GetComponent<StarBehaviour>().Seed = starSeed;
            obj.GetComponent<StarBehaviour>().Sector = this;
            obj.AddComponent<StarSystem>();
            stars.Add(obj);
            starSeed++;
        }
    }
}
