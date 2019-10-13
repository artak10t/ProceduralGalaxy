using System.Collections.Generic;
using UnityEngine;

/*
Generates a volumentic galaxy based on the passed bitmap, using the partcile system.

 COORDINATES:

                  +Z           -Y  <Flying below the> 
                   |         /    <plane>
                   |       /
                   |     /
                   |   /
             <Galactic Plane>
-X ------ <Viewed from 'above'> ------ +X
               /   |
             /     |
           /       |
         /         |
        +Y        -Z
  <Flying above>
  <the plane>

Developed by Natasha CARL, @Imifos
License: Public domain, use at your own risk.
*/
public class GalaxyCloudForge : MonoBehaviour
{
    public bool fakeStars = true;
    // Galaxy dimensions
    // Result of trial and error by visually matching the EDSM.net star data onto
    // the generated galaxy plane. EDSM star data is (0,0,0) on Sol.
    // Note that this star data visualisation is not part of this demo.
    private readonly float galaxyXBase = -450f; // left 
    private readonly float galaxyZBase = -170f; // bottom
    private readonly float galaxyYBase = 0f;    // Galactic plane

    // Scale factor bitmap size (1 pixel unit) to galaxy size (1.7 Unity3D units)
    // Same process as above to find these values. Increase to have a bigger galaxy.
    private readonly float galaxyUnityPerPixelWidth = 1.7f;
    private readonly float galaxyUnityPerPixelHeight = 1.7f;

    // Assigned galaxy bitmap in Inspector
    // The galaxy bitmap used in this demo is from my favorite Space Sandbox
    // Elite Dangerous, (c) Frontier Development
    public Texture2D galaxyBitmap;

    // Particles added to the particle system of the current transform.
    // It should use a custom material set to the default (Unity3D) Particle bitmap
    // but the Built-in "Additive (Soft)" shader.
    private List<ParticleSystem.Particle> particles = new List<ParticleSystem.Particle>();

    private float textureWidth;
    private float textureHeight;
    private float textureCenterXY;

    /*
     * Generates a galaxy as cloud of particles, based on a template bitmap.
     */
    private void BuildGalaxy()
    {
        textureWidth = galaxyBitmap.width;
        textureHeight = galaxyBitmap.height;
        textureCenterXY = textureWidth / 2; // bitmap is nearly a square, so one value fits X and Y

        particles.Clear(); // In case we call the method in a loop for debugging

        // Generate large clouds
        // ---------------------
        for (float y = 0; y < textureHeight; y += 9)
            for (float x = 0; x < textureWidth; x += 9)
            {
                Color c = galaxyBitmap.GetPixel((int)x, (int)y);

                if (c.g < 0.01)
                    continue;

                float px = galaxyXBase + galaxyUnityPerPixelWidth * x;
                float pz = galaxyZBase + galaxyUnityPerPixelHeight * y;

                px += Random.Range(-5, 6);
                pz += Random.Range(-7, 5);

                // 0 at the borders, 1 on one of the center axes. It forms a 'square',
                // not a circle (around the center), so it's really a huge approximation.
                // Used (below) to generate larger clouds in the center, but brighter clouds 
                // outside to galactic arms better visible.
                float axisDistanceFactor = 1 - (Mathf.Abs(x - textureCenterXY) / textureWidth +
                                                Mathf.Abs(y - textureCenterXY) / textureWidth);

                // Small emphasis on "blue only" sectors in terms of luminosity
                float blueFactor = 0f;
                if (c.r < 0.2 && c.g < 0.3 && c.g > 0.01)
                    blueFactor = (1 - c.b) / 4;

                ParticleSystem.Particle p = new ParticleSystem.Particle
                {
                    position = new Vector3(px, galaxyYBase, pz),
                    startSize = Random.Range(40 + 100 * axisDistanceFactor, 70 + 150 * axisDistanceFactor),
                    startColor = new Color(c.r, c.g, c.b, 0.05f + (1.0f - axisDistanceFactor) * 0.3f + blueFactor)
                };

                particles.Add(p);
            }

        // NOTE: we could do both operations in one loop and remove some duplicated code.
        // For educational purpose, the 2 operations are in separate bitmap scan loops.
        // To gain performance, GetPixel() should be replaced by GetPixels32().

        // Generate bright small clouds and 'fake' stars
        // ---------------------------------------------
        int attractor = 0;
        for (float y = 0; y < textureHeight; y += 5)
            for (float x = 0; x < textureWidth; x += 5)
            {
                Color c = galaxyBitmap.GetPixel((int)x, (int)y);

                // See above
                // Used as above, except for making brighter clouds in the center.
                // Also used to adapt the Y position (above or below the galactic plane) of the clouds.
                float axisDistanceFactor = 1 - (Mathf.Abs(x - textureCenterXY) / textureWidth +
                                                Mathf.Abs(y - textureCenterXY) / textureWidth);

                float px = galaxyXBase + galaxyUnityPerPixelWidth * x;
                float pz = galaxyZBase + galaxyUnityPerPixelHeight * y;
                float py = Random.Range(-10 - axisDistanceFactor * 30, 10 + axisDistanceFactor * 30);

                // Clouds
                // Randomised on bitmap points which have a given color level
                // Note: c.g = grayscale value = brightness, which is not useful here
                // as this would be centered in the bright galaxy center.
                float rnd = Random.Range(0, Mathf.Clamp(20 - attractor, 1, 20));
                if ((c.r > 0.2f || c.g > 0.2f || c.b > 0.2f) && rnd < 2f)
                {
                    // Increase chance to build groups of clouds, up to 3 times
                    attractor += 7;
                    if (attractor > 25)
                        attractor = 0;

                    float blueFactor = 0f;
                    if (c.r < 0.2 && c.g < 0.5)
                        blueFactor = (1 - c.b) / 3;

                    ParticleSystem.Particle p = new ParticleSystem.Particle
                    {
                        position = new Vector3(px, galaxyYBase + py, pz),
                        startSize = Random.Range(5 + axisDistanceFactor * 20, 80 + axisDistanceFactor * 120),
                        startColor = new Color(c.r, c.g, c.b, Random.Range(0.1f, 0.3f * axisDistanceFactor + blueFactor))
                    };

                    particles.Add(p);
                }
                else
                {
                    // No new cloud generated, reset random() probability
                    attractor = 0;
                }

                // Stars
                if (fakeStars)
                {
                    rnd = Random.Range(0, 50);
                    if ((c.r > 0.4f || c.g > 0.2f || c.b > 0.2f) && rnd < 2.5)
                    {
                        py = Random.Range(-20 - axisDistanceFactor * 40, 20 + axisDistanceFactor * 50);
                        ParticleSystem.Particle p2 = new ParticleSystem.Particle
                        {
                            position = new Vector3(px, galaxyYBase + py, pz),
                            startSize = Random.Range(2, 5),
                            startColor = new Color(1, 1, 1, Random.Range(0.4f, 0.7f))
                        };
                        particles.Add(p2);
                    }
                }
            }

        // And finally: all partciles into the scene
        GetComponent<ParticleSystem>().SetParticles(particles.ToArray(), particles.Count);

        //Debug.Log("Galaxy - Added Particles:" + particles.Count);
    }

    // Unity3D call back at scene start
    void Start()
    {
        BuildGalaxy();
    }

    // Unty3D call back for every frame
    private float nextActionTime = 0.0f;
    private readonly float period = 3f;
    void Update()
    {
        // Debugging and testing
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            //BuildGalaxy();
        }
    }
}
