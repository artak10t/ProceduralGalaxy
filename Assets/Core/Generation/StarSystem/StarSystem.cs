using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem : MonoBehaviour
{
    public StarBehaviour star;

    private Transform cam;
    private List<GameObject> planets = new List<GameObject>();
    private float generationDistance = 250f;
    private bool generated = false;

    private void Start()
    {
        cam = Camera.main.transform;
        star = GetComponent<StarBehaviour>();
        generationDistance = star.Scale * 5f;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, cam.position) < generationDistance && !generated)
        {
            generated = true;
            GeneratePlanets();
        }
        else if (Vector3.Distance(transform.position, cam.position) > generationDistance && generated)
        {
            generated = false;
            DestroyPlanets();
        }
    }

    private void DestroyPlanets()
    {
        Destroy(star.model.GetComponent<Light>());
        for (int i = 0; i < planets.Count; i++)
        {
            Destroy(planets[i]);
        }
    }

    private void GeneratePlanets()
    {
        Light light = star.model.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 1000000000;
        light.intensity = 100000000;

        Random.InitState(star.Seed);
        int planetsCount = Random.Range(0, 8);

        for (int i = 0; i < planetsCount; i++)
        {
            GameObject planet = new GameObject("Planet: " + star.Name + "-" + i);
            planet.transform.parent = transform;
            planet.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Primitives/CubeSphere", typeof(Mesh));
            planet.AddComponent<MeshRenderer>();

            planet.GetComponent<Renderer>().material = (Material)Resources.Load("Planets/PlanetModelMaterial", typeof(Material));
            planet.GetComponent<Renderer>().material.SetFloat("Vector1_103A71D3", Random.value * star.Seed);
            Color color = new Color(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1), 1);
            Color atmoColor = new Color(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1), 1);
            Color liquColor = new Color(Random.Range(0.1f, 1), Random.Range(0.1f, 1), Random.Range(0.1f, 1), 1);
            planet.GetComponent<Renderer>().material.SetColor("Color_976DB11", color);
            planet.GetComponent<Renderer>().material.SetColor("Color_33D1831D", atmoColor);
            planet.GetComponent<Renderer>().material.SetColor("Color_40A00648", liquColor);

            planet.AddComponent<Orbit>().focusObject = transform;

            //Distance from star
            float distance = Random.Range(3.00f, 75.00f) * (star.Scale / 50);
            planet.GetComponent<Orbit>().semiMajorAxis = distance;

            //Scale
            float scale = Random.Range(100.00f, 350.00f);
            planet.transform.localScale = transform.localScale / (star.Scale * 10f);

            //Eccentricity
            float eccentricity = Random.Range(0.00f, 0.30f);
            planet.GetComponent<Orbit>().eccentricity = eccentricity;

            //Orbit Rotation
            float x = Random.Range(-10.00f, 10.00f);
            float y = Random.Range(-10.00f, 10.00f);
            float z = Random.Range(-10.00f, 10.00f);
            planet.GetComponent<Orbit>().rotation = new Vector3(x, y, z);

            float velocity = star.Scale / 100 / distance;

            //Orbit StartPosition
            float startPosition = Random.Range(0.00f, 6.00f);
            planet.GetComponent<Orbit>().startPosition = startPosition + (GlobalSettings.singleton.CurrentTime * velocity);

            planet.GetComponent<Orbit>().orbitalVelocity = velocity;
            planets.Add(planet);
        }
    }
}
