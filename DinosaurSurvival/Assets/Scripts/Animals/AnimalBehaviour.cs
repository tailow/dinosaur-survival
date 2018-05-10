using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimalBehaviour : MonoBehaviour {

	public string species;

	public List<string> spawnLocations;

	GameObject player;

	int speed;
	int targetSpeed;

	int acceleration;
	int targetAcceleration;

	public int health;
	public int fleeHealth;
	public int walkSpeed;
	public int sprintSpeed;
	public int damage;
	public int spawnAmount;
	public int walkAcceleration;
	public int sprintAcceleration;

	public bool isAggressive;
	public bool isNeutral;
	public bool isPassive;

	bool isFleeing = false;
	bool isAttacking = false;

	public float detectionRange;
	public float sightRange;

	NavMeshAgent navMeshAgent;

	void Update ()
	{
		speed = targetSpeed;
		acceleration = targetAcceleration;

		navMeshAgent.speed = speed;
		navMeshAgent.acceleration = acceleration;

		if (Vector3.Distance(transform.position, player.transform.position) > sightRange && isFleeing && !isAttacking)
		{
			StopAllCoroutines();

			StartCoroutine(Wander());
		}

		if (Vector3.Distance(player.transform.position, transform.position) > sightRange && !isFleeing && isAttacking)
		{
			StopAllCoroutines();

			StartCoroutine(Wander());
		}
	}

	public void TakeDamage(int amountOfDamage)
	{
		health -= amountOfDamage;

		if (health < 0)
		{
			Destroy(gameObject);
		}

		else if (health < fleeHealth && !isFleeing)
		{
			StopAllCoroutines();

			StartCoroutine(Flee());
		}

		else if (isNeutral && health > fleeHealth && !isAttacking)
		{
			StartCoroutine(Attack());
		}
	}

	IEnumerator Attack()
	{
		isFleeing = false;
		isAttacking = true;

		targetAcceleration = sprintAcceleration;
		targetSpeed = sprintSpeed;

		gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
		
		for (int i = 0; i < 10000; i++)
		{
			Vector3 dir = player.transform.position - transform.position;
			Vector3 nextPos = transform.position + dir.normalized;

			navMeshAgent.SetDestination(CalculatePosition(nextPos.x, nextPos.z));

			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator Wander()
	{
		isFleeing = false;
		isAttacking = false;

		targetSpeed = walkSpeed;
		targetAcceleration = walkAcceleration;

		gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

		for (int i = 0; i < 10000; i++)
		{
			Vector3 nextPos = CalculatePosition(transform.position.x + Random.Range(-30f, 30f), transform.position.z + Random.Range(-30f, 30f));

			navMeshAgent.SetDestination(nextPos);

			yield return new WaitForSeconds(6f);
		}
	}

	IEnumerator Flee()
	{
		isFleeing = true;
		isAttacking = false;

		targetAcceleration = sprintAcceleration;
		targetSpeed = sprintSpeed;

		gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
		
		for (int i = 0; i < 10000; i++)
		{
			Vector3 dir = transform.position - player.transform.position;
			Vector3 nextPos = transform.position + dir.normalized * 10;

			navMeshAgent.SetDestination(CalculatePosition(nextPos.x, nextPos.z));

			yield return new WaitForEndOfFrame();
		}
	}

	Vector3 CalculatePosition(float x, float z)
	{
		Vector3 rayPosition = new Vector3(x, 100f, z);

		RaycastHit hit;
		Ray ray = new Ray(rayPosition, Vector3.down);

		if (Physics.Raycast(ray, out hit, 100f))
		{
			Vector3 desiredPos = hit.point;

			NavMeshHit closestHit;

			if (NavMesh.SamplePosition(desiredPos, out closestHit, 100f, 1))
			{
				return closestHit.position;
			}

			else
			{
				return transform.position;
			}
		}

		else
		{
			return transform.position;
		}
	}

	public void AddNavMeshAgent()
	{
		navMeshAgent = gameObject.AddComponent<NavMeshAgent>();

		targetSpeed = walkSpeed;
		targetAcceleration = walkAcceleration;

		StartCoroutine(Wander());

		player = Camera.main.transform.parent.gameObject;
	}
}
