using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public int width = 100;
    public int height = 100;
    public float scale = 20f;

    public string seed;
    public bool random;

    float[,] noiseMap;

    const int VILLAGEAREASIZE = 80;
    void Start()
    {
        float size = GetComponent<Terrain>().terrainData.size.x / 2;
        width = 100 * ((int)size / 500);
        height = 100 * ((int)size / 500);

        noiseMap = new float[width, height];

        if (!random)
            Random.InitState(seed.GetHashCode());

        //StartCoroutine(GenerateWorld());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord + Random.Range(0, 100000), yCoord + Random.Range(0, 100000));
                noiseMap[x, y] = sample;

                //trees
                if (noiseMap[x, y] < 0.3f)
                {
                    if (Random.Range(0, 100) < 20)
                    {
                        //if (!(x * 10 - size > -VILLAGEAREASIZE && x * 10 - size < VILLAGEAREASIZE && y * 10 - size > -VILLAGEAREASIZE && y * 10 - size < VILLAGEAREASIZE))
                        {
                            GameObject a = Instantiate(GameManager.instance.tree);
                            a.transform.position = new Vector3(x * 10 - size, 0, y * 10 - size);
                            a.transform.parent = transform;
                        }
                    }
                }

                //irons
                if (noiseMap[x, y] > 0.9f)
                {
                    if (Random.Range(0, 100) < 20)
                    {
                        if (!(x * 10 - size > -VILLAGEAREASIZE && x * 10 - size < VILLAGEAREASIZE && y * 10 - size > -VILLAGEAREASIZE && y * 10 - size < VILLAGEAREASIZE))
                        {
                            GameObject a = Instantiate(GameManager.instance.ironGroup);
                            a.transform.position = new Vector3(x * 10 - size, 0, y * 10 - size);
                            a.transform.parent = transform;
                        }
                    }
                }

                //enemy groups
                if (noiseMap[x, y] > 0.9f)
                {
                    if (Random.Range(0, 100) < 20)
                    {
                        if (!(x * 10 - size > -VILLAGEAREASIZE * 3 && x * 10 - size < VILLAGEAREASIZE * 3 && y * 10 - size > -VILLAGEAREASIZE * 3 && y * 10 - size < VILLAGEAREASIZE * 3))
                        {
                            GameObject a = Instantiate(GameManager.instance.goblinGroup);
                            a.transform.position = new Vector3(x * 10 - size, 0, y * 10 - size);
                            a.transform.parent = transform;
                        }
                    }
                }
            }
        }
    }
    IEnumerator GenerateWorld()
    {
        yield return new WaitForSeconds(0);
    }
}
