using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalSpawner : MonoBehaviour {
	
	public MapGenerator mapGenerator;

	public GameObject[] animalPrefabs;

	NavMeshHit closestHit;

	public Transform animalParent;

	public void SpawnAnimals()
	{
		for (int x = 0; x < animalPrefabs.Length; x++)
		{
			List<Vector3> spawnList = ListAllSpawnPoints(animalPrefabs[x]);

			for (int i = 0; i < animalPrefabs[x].GetComponent<AnimalBehaviour>().spawnAmount; i++)
			{
				SpawnAnimal(animalPrefabs[x], spawnList);
			}
		}
	}

	void SpawnAnimal(GameObject animalPrefab, List<Vector3> spawnList)
	{
		Vector3 desiredPosition = spawnList[Random.Range(0, spawnList.Count)];

		GameObject animal = Instantiate(animalPrefab, desiredPosition, Quaternion.identity);

		animal.transform.parent = animalParent;

		animal.SendMessage("AddNavMeshAgent");
	}

	List<Vector3> ListAllSpawnPoints(GameObject animalPrefab)
	{
		List<Vector3> spawnPointList = new List<Vector3>();

		for (int y = 0; y > -mapGenerator.mapSize; y--)
        {
            for (int x = 0; x < mapGenerator.mapSize; x++)
			{
				RaycastHit hit;
				Ray ray = new Ray(new Vector3(x, 100, y), Vector3.down);

				if (Physics.Raycast(ray, out hit))
				{
					if (IsAtCorrectAltitude(animalPrefab, hit))
					{
						spawnPointList.Add(hit.point);
					}
				}
			}
		}

		return spawnPointList;
	}

	bool IsAtCorrectAltitude(GameObject animalPrefab, RaycastHit hit)
	{
		List<Biome> biomeList = new List<Biome>();

		bool isAtCorrectAlt = false;

		biomeList.Add(new Biome("Beach", -2.3f, 1.63f));
		biomeList.Add(new Biome("Plains", 1.63f, 4f));
		biomeList.Add(new Biome("Forest", 4.2f, 8f));
		biomeList.Add(new Biome("Mountains", 8f, 40f));

		for (int i = 0; i < animalPrefab.GetComponent<AnimalBehaviour>().spawnLocations.Count; i++)
		{
			for (int y = 0; y < biomeList.Count; y++)
			{
				if (biomeList[y].biomeName == animalPrefab.GetComponent<AnimalBehaviour>().spawnLocations[i])
				{
					if (hit.point.y > biomeList[y].minHeight && hit.point.y < biomeList[y].maxHeight)
					{
						isAtCorrectAlt = true;
					}
				}
			}
		}

		return isAtCorrectAlt;
	}
}

public struct Biome
{
	public string biomeName;

	public float minHeight;
	public float maxHeight;

	public Biome(string name, float minimumHeight, float maximumHeight)
	{
		biomeName = name;
		minHeight = minimumHeight;
		maxHeight = maximumHeight;
	}
}