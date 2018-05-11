﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour
{
    #region Variables

    public GameManager gameManager;
    public GameObject player;

    int offsetX;
    int offsetY;
    int pointX;
    int pointY;   
    int amountOfChunksPerLine;

    const int chunkSize = 96;

    public Region[] regions;

    public GameObject chunkPrefab;

    public SpawnableObjectGroup[] spawnableObjectGroups;

    List<Vector3> spawnPoints;

    public Transform terrainParent;

    public NavMeshSurface navMesh;

    float minPointHeight;
    float maxPointHeight;

    public float depth;
    public float scale;
    public float lacunarity;
    public float fallOffStrength;
    public float fallOffStart;

    [Range(0, 1)]
    public float persistance;

    public int mapSize;
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
        seed = Random.Range(-5000, 5000);

        GenerateChunks();

        navMesh.RemoveData();
        navMesh.BuildNavMesh();

        gameManager.GetComponent<AnimalSpawner>().SpawnAnimals();
    }

    public void GenerateChunks()
    {
        DeleteObjects();

        GameObject chunkParentObject = new GameObject();
        Transform chunkParent = chunkParentObject.transform;

        chunkParent.transform.parent = terrainParent;
        chunkParent.name = "Chunks";

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
       
        GenerateSpawnableObjects();

        GenerateSpawnPoints();
    }

    public void DeleteObjects()
    {
        int amountOfGroups = terrainParent.childCount;

        List<GameObject> destroyableObjects = new List<GameObject>();

        for (int i = 0; i < amountOfGroups; i++)
        {
            if (terrainParent.GetChild(i).name != "Seafloor")
            {
                destroyableObjects.Add(terrainParent.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < destroyableObjects.Count; i++)
        {
            DestroyImmediate(destroyableObjects[i]);
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

    public void GenerateSpawnPoints()
    {
        spawnPoints = new List<Vector3>();

        for (int y = 0; y > -mapSize; y--)
        {
            for (int x = 0; x < mapSize; x++)
            {
                RaycastHit hit;
                Ray ray = new Ray(new Vector3(x, 200f, y), Vector3.down);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Ground" && hit.point.y < 1.5f && hit.point.y > 0.1f)
                    {
                        spawnPoints.Add(hit.point);
                    }
                }
            }
        }

        player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)];
    }

    public void GenerateSpawnableObjects()
    {
        for (int i = 0; i < spawnableObjectGroups.Length; i++)
        {
            SpawnableObjectGroup objectGroup = spawnableObjectGroups[i];

            GameObject objectGroupParent = new GameObject();

            objectGroupParent.transform.parent = terrainParent;
            objectGroupParent.name = objectGroup.name;

            for (float y = 0; y > -mapSize; y -= (objectGroup.positionRandomness + objectGroup.positionGap))
            {
                for (float x = 0; x < mapSize; x += (objectGroup.positionRandomness + objectGroup.positionGap))
                {
                    float objectSpawnX = Random.Range(x, x + objectGroup.positionRandomness);
                    float objectSpawnZ = Random.Range(y, y - objectGroup.positionRandomness);
                    float objectSpawnY = -100f;

                    List<SpawnableObject> allPossibleObjects = new List<SpawnableObject>();

                    RaycastHit hit;
                    Ray ray = new Ray(new Vector3(objectSpawnX, 200f, objectSpawnZ), Vector3.down);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.gameObject.tag == "Ground")
                        {
                            objectSpawnY = hit.point.y;
                        }
                    }

                    Vector3 objectSpawnPosition = new Vector3(objectSpawnX, objectSpawnY, objectSpawnZ);

                    for (int j = 0; j < spawnableObjectGroups[i].objectArray.Length; j++)
                    {
                        SpawnableObject possibleObject = spawnableObjectGroups[i].objectArray[j];

                        if (objectSpawnY > possibleObject.startHeight && objectSpawnY < possibleObject.endHeight)
                        {
                            allPossibleObjects.Add(possibleObject);
                        }
                    }

                    if (allPossibleObjects.Count > 0)
                    {
                        SpawnableObject spawnableObject = allPossibleObjects[Random.Range(0, allPossibleObjects.Count)];

                        GameObject spawnedObject = Instantiate(spawnableObject.prefab, objectSpawnPosition, spawnableObject.prefab.transform.localRotation);

                        spawnedObject.transform.Translate(new Vector3(0, spawnableObject.depth, 0));
                        spawnedObject.transform.localScale *= Random.Range(1 - spawnableObject.sizeVariation, 1 + spawnableObject.sizeVariation);

                        spawnedObject.transform.Rotate(Vector3.up * Random.Range(0, 360), Space.World);

                        spawnedObject.transform.parent = objectGroupParent.transform;
                    }                  
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

[System.Serializable]
public struct SpawnableObject
{
    public string name;

    public float depth;
    public float startHeight;
    public float endHeight;
    public float sizeVariation;

    public GameObject prefab;
}

[System.Serializable]
public struct SpawnableObjectGroup
{
    public string name;

    public float positionRandomness;
    public float positionGap;

    public SpawnableObject[] objectArray;
}