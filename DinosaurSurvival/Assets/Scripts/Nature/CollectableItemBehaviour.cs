using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItemBehaviour : MonoBehaviour {

	#region Variables
	
	public int health;

	#endregion

	public void TakeDamage(int amount)
	{
		health -= amount;

		if (health <= 0)
		{
			Destroy(gameObject);
		}
	}
}
