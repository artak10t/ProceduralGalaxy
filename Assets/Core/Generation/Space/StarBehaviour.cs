using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBehaviour : MonoBehaviour
{
    public Sector Sector;
    public float Temperature;
    public float Mass;
    public float Scale;
    public int Seed;
    public string Name;
    public GameObject model;

    private Transform _cam;
    private float modelRenderDistance = 500f;
    private bool modelRendered = false;

    private void Start()
    {
        modelRenderDistance = transform.localScale.x * 100f;
        _cam = Camera.main.transform;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _cam.position) < modelRenderDistance && !modelRendered)
        {
            modelRendered = true;
            CreateStarModel();
        }
        else if (Vector3.Distance(transform.position, _cam.position) > modelRenderDistance && modelRendered)
        {
            modelRendered = false;
            DestroyStarModel();
        }
    }

    private void DestroyStarModel()
    {
        Destroy(model);
    }

    private void CreateStarModel()
    {
        //3dModel
        model = new GameObject("StarModel: " + name);
        model.transform.parent = transform;
        model.transform.position = transform.position;
        model.transform.localScale = transform.localScale / Scale;
        model.AddComponent<MeshFilter>().mesh = (Mesh)Resources.Load("Primitives/CubeSphere", typeof(Mesh));
        model.AddComponent<MeshRenderer>();
        model.GetComponent<Renderer>().material = (Material)Resources.Load("Stars/StarModelMaterial", typeof(Material));
        Color color = Mathf.CorrelatedColorTemperatureToRGB(Temperature);
        model.GetComponent<Renderer>().material.SetColor("Color_976DB11", color);
    }
}
