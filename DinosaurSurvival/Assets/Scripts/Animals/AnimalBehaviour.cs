using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBehaviour : MonoBehaviour {

	public string species;

	public List<string> spawnLocations;

	int speed;
	int targetSpeed;

	public int health;
	public int fleeHealth;
	public int walkingSpeed;
	public int sprintingSpeed;
	public int damage;
	public int spawnAmount;
	public int acceleration;

	public bool isAggressive;
	public bool isNeutral;
	public bool isPassive;

	public float detectionRange;
	public float sightRange;

	NavMeshAgent navMeshAgent;

	void Update ()
	{
		speed = targetSpeed;

		navMeshAgent.speed = speed;	
	}

	public void AddNavMeshAgent()
	{
		navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
		
		navMeshAgent.acceleration = acceleration;

		targetSpeed = walkingSpeed;

		StartCoroutine(Wander());
	}

	IEnumerator Wander()
	{		
		for	(int i = 0; i < 10000; i++)
		{
			RaycastHit hit;

			Vector3 randomPosition = new Vector3(transform.position.x + Random.Range(-40, 40), transform.position.y + 50, transform.position.z + Random.Range(-40, 40));
			Vector3 nextDestination = transform.position;

			Ray ray = new Ray(randomPosition, Vector3.down);

			if (Physics.Raycast(ray, out hit, 100f))
			{
				nextDestination = hit.point;
			}

			navMeshAgent.SetDestination(nextDestination);

			yield return new WaitForSeconds(6f);
		}
	}

	void Flee()
	{
		Sprint();
	}

	void Sprint()
	{
		targetSpeed = sprintingSpeed;
	}
}
