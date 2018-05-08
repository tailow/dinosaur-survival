using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Variables

    int offsetX;
    int offsetY;
    int pointX;
    int pointY;
    int mapSize;
    int amountOfChunksPerLine;
    int amountOfTrees;
    int amountOfRocks;
    int amountOfGrass;

    const int chunkSize = 96;

    public Region[] regions;

    public GameObject chunkPrefab;

    public GameObject[] treePrefabs;
    public GameObject[] rockPrefabs;
    public GameObject[] grassPrefabs;

    public Transform chunkParent;
    public Transform treeParent;
    public Transform rockParent;
    public Transform grassParent;

    float minPointHeight;
    float maxPointHeight;

    public float depth;
    public float scale;
    public float lacunarity;
    public float fallOffStrength;
    public float fallOffStart;

    public float treePositionRandomness;
    public float treePositionGap;
    public float treeStartHeight;
    public float treeEndHeight;

    public float rockPositionRandomness;
    public float rockPositionGap;
    public float rockStartHeight;
    public float rockEndHeight;

    public float grassPositionRandomness;
    public float grassPositionGap;
    public float grassStartHeight;
    public float grassEndHeight;

    [Range(0, 1)]
    public float persistance;

    public int octaves;
    public int seed;
    public int amountOfChunks;

    [Range(0, 6)]
    public int levelOfDetail;

    public bool autoUpdate;

    public AnimationCurve meshHeightCurve;

    #endregion

    void Start()
    {
        GenerateChunks();

        seed = Random.Range(-5000, 5000);
    }

    public void GenerateChunks()
    {
        DeleteChunks();

        amountOfChunksPerLine = (int)Mathf.Sqrt(amountOfChunks);

        mapSize = chunkSize * amountOfChunksPerLine;

        System.Random rng = new System.Random(seed);

        offsetX = rng.Next(-50000, 50000);
        offsetY = rng.Next(-50000, 50000);

        if (chunkParent.childCount == 0)
        {
            for (int y = 0; y < amountOfChunksPerLine; y++)
            {
                for (int x = 0; x < amountOfChunksPerLine; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab, new Vector3((x * chunkSize) + (chunkSize / 2), 0, (-y * chunkSize) - (chunkSize / 2)), Quaternion.identity);

                    chunk.transform.parent = chunkParent;
                }
            }
        }

        for (int i = 0; i < chunkParent.childCount; i++)
        {
            GenerateTerrain(chunkParent.GetChild(i));
        }

        DeleteTrees();
        DeleteRocks();
        DeleteGrass();

        GenerateTrees();
        GenerateRocks();
        GenerateGrass();
    }

    public void DeleteChunks()
    {
        if (chunkParent.childCount > 0)
        {
            for (int i = 0; i < amountOfChunks; i++)
            {
                DestroyImmediate(chunkParent.GetChild(0).gameObject);
            }
        }
    }

    public void DeleteTrees()
    {
        amountOfTrees = treeParent.childCount;

        if (treeParent.childCount > 0)
        {
            for (int i = 0; i < amountOfTrees; i++)
            {
                DestroyImmediate(treeParent.GetChild(0).gameObject);
            }
        }
    }

    public void DeleteRocks()
    {
        amountOfRocks = rockParent.childCount;

        if (rockParent.childCount > 0)
        {
            for (int i = 0; i < amountOfRocks; i++)
            {
                DestroyImmediate(rockParent.GetChild(0).gameObject);
            }
        }
    }

    public void DeleteGrass()
    {
        amountOfGrass = grassParent.childCount;

        if (grassParent.childCount > 0)
        {
            for (int i = 0; i < amountOfGrass; i++)
            {
                DestroyImmediate(grassParent.GetChild(0).gameObject);
            }
        }
    }

    public void GenerateTerrain(Transform chunk)
    {
        MeshFilter terrainMesh = chunk.GetComponent<MeshFilter>();

        terrainMesh.mesh = MeshGenerator.GenerateTerrainMesh(GenerateNoiseMap(chunk, GenerateFallOffMap(chunk)), meshHeightCurve, levelOfDetail, depth, GenerateFallOffMap(chunk)).CreateMesh();

        var tempMaterial = new Material(terrainMesh.gameObject.GetComponent<MeshRenderer>().sharedMaterial);

        terrainMesh.gameObject.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;

        tempMaterial.mainTexture = GenerateColorMap(chunk, terrainMesh, GenerateFallOffMap(chunk));

        chunk.GetComponent<MeshCollider>().sharedMesh = terrainMesh.sharedMesh;
    }

    public void GenerateTrees()
    {
        for (float y = 0; y > -mapSize; y -= (treePositionRandomness + treePositionGap))
        {
            for (float x = 0; x < mapSize; x += (treePositionRandomness + treePositionGap))
            {
                float treeSpawnX = Random.Range(x, x + treePositionRandomness);
                float treeSpawnZ = Random.Range(y, y - treePositionRandomness);
                float treeSpawnY;

                RaycastHit hit;
                Ray ray = new Ray(new Vector3(treeSpawnX, 200f, treeSpawnZ), Vector3.down);

                if (Physics.Raycast(ray, out hit))
                {
                    treeSpawnY = hit.point.y;
                }

                else
                {
                    treeSpawnY = 10f;
                }

                Vector3 treeSpawnPosition = new Vector3(treeSpawnX, treeSpawnY - 0.5f, treeSpawnZ);

                if (treeSpawnY > treeStartHeight && treeSpawnY < treeEndHeight)
                {
                    GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];

                    GameObject tree = Instantiate(treePrefab, treeSpawnPosition, treePrefab.transform.rotation);

                    tree.transform.localScale *= Random.Range(0.9f, 1f);

                    tree.transform.parent = treeParent;
                }
            }
        }
    }

    public void GenerateRocks()
    {
        for (float y = 0; y > -mapSize; y -= (rockPositionRandomness + rockPositionGap))
        {
            for (float x = 0; x < mapSize; x += (rockPositionRandomness + rockPositionGap))
            {
                float rockSpawnX = Random.Range(x, x + rockPositionRandomness);
                float rockSpawnZ = Random.Range(y, y - rockPositionRandomness);
                float rockSpawnY = -100f;

                RaycastHit hit;
                Ray ray = new Ray(new Vector3(rockSpawnX, 200f, rockSpawnZ), Vector3.down);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Ground")
                    {
                        rockSpawnY = hit.point.y;
                    }
                }

                Vector3 rockSpawnPosition = new Vector3(rockSpawnX, rockSpawnY - 1f, rockSpawnZ);

                if (rockSpawnY > rockStartHeight && rockSpawnY < rockEndHeight)
                {
                    GameObject rockPrefab = rockPrefabs[Random.Range(0, rockPrefabs.Length)];

                    GameObject rock = Instantiate(rockPrefab, rockSpawnPosition, rockPrefab.transform.rotation);

                    rock.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0));

                    rock.transform.localScale *= Random.Range(0.2f, 1.5f);

                    rock.transform.parent = rockParent;
                }
            }
        }
    }

    public void GenerateGrass()
    {
        for (float y = 0; y > -mapSize; y -= (grassPositionRandomness + grassPositionGap))
        {
            for (float x = 0; x < mapSize; x += (grassPositionRandomness + grassPositionGap))
            {
                float grassSpawnX = Random.Range(x, x + grassPositionRandomness);
                float grassSpawnZ = Random.Range(y, y - grassPositionRandomness);
                float grassSpawnY = -100f;

                RaycastHit hit;
                Ray ray = new Ray(new Vector3(grassSpawnX, 200f, grassSpawnZ), Vector3.down);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Ground")
                    {
                        grassSpawnY = hit.point.y;
                    }
                }

                Vector3 grassSpawnPosition = new Vector3(grassSpawnX, grassSpawnY, grassSpawnZ);

                if (grassSpawnY > grassStartHeight && grassSpawnY < grassEndHeight)
                {
                    GameObject grassPrefab = grassPrefabs[Random.Range(0, grassPrefabs.Length)];

                    GameObject grass = Instantiate(grassPrefab, grassSpawnPosition, grassPrefab.transform.rotation);

                    grass.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0));

                    grass.transform.localScale *= Random.Range(0.9f, 1f);

                    grass.transform.parent = grassParent;
                }
            }
        }
    }

    float[,] GenerateNoiseMap(Transform chunk, float[,] fallOffMap)
    {
        float[,] noiseMap = new float[chunkSize + 1, chunkSize + 1];

        for (int y = 0; y < chunkSize + 1; y++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                noiseMap[x, y] = Mathf.Clamp01(GetPointHeight(new Vector2(x, y), chunk) - fallOffMap[x, y]);
            }
        }

        return noiseMap;
    }

    float GetPointHeight(Vector2 point, Transform chunk)
    {
        float pointHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        pointX = (int)chunk.position.x + (int)point.x - (chunkSize / 2);
        pointY = (int)chunk.position.z - (int)point.y - (chunkSize / 2);

        float sampleX;
        float sampleY;

        float maxPossibleHeight = 0;

        for (int i = 0; i < octaves; i++)
        {
            sampleX = pointX / scale * frequency + offsetX;
            sampleY = pointY / scale * frequency + offsetY;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            pointHeight += perlinValue * amplitude;

            maxPossibleHeight += amplitude;

            frequency *= lacunarity;
            amplitude *= persistance;
        }

        pointHeight = pointHeight / maxPossibleHeight;

        return pointHeight;
    }

    public float[,] GenerateFallOffMap(Transform chunk)
    {
        float[,] fallOffMap = new float[chunkSize + 1, chunkSize + 1];

        for (int y = 0; y < chunkSize + 1; y++)
        {
            for (int x = 0; x < chunkSize + 1; x++)
            {
                int pointX = Mathf.Abs((int)chunk.position.x + x - (chunkSize / 2));
                int pointY = Mathf.Abs((int)chunk.position.z - y + (chunkSize / 2));

                float distanceFromEdge = Mathf.Max(Mathf.Abs(pointX / (float)mapSize * 2 - 1), Mathf.Abs(pointY / (float)mapSize * 2 - 1));

                float a = fallOffStrength;
                float b = fallOffStart;

                float fallOffMultiplier = Mathf.Pow(distanceFromEdge, a) / (Mathf.Pow(distanceFromEdge, a) + Mathf.Pow(b - b * distanceFromEdge, a));

                fallOffMap[x, y] = fallOffMultiplier;
            }
        }

        return fallOffMap;
    }

    Texture2D GenerateColorMap(Transform chunk, MeshFilter terrainMesh, float[,] fallOffMap)
    {
        int increment = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;

        int colorMapSize = (chunkSize + 1) / increment;

        Texture2D colorTexture = new Texture2D(colorMapSize, colorMapSize);

        Color[] colorMap = new Color[colorMapSize * colorMapSize];

        for (int y = 0; y < colorMapSize; y++)
        {
            for (int x = 0; x < colorMapSize; x++)
            {
                for (int i = 0; i < regions.Length; i++)
                {
                    float pointHeight = meshHeightCurve.Evaluate(Mathf.Clamp01((GetPointHeight(new Vector2(x * increment, y * increment), chunk)) - fallOffMap[x, y])) * depth;

                    if (pointHeight >= regions[i].startHeight)
                    {
                        colorMap[y * colorMapSize + x] = regions[i].color;
                    }
                }
            }
        }

        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.SetPixels(colorMap);
        colorTexture.Apply();

        return colorTexture;
    }
}

[System.Serializable]
public struct Region
{
    public string name;
    public float startHeight;
    public Color color;
}