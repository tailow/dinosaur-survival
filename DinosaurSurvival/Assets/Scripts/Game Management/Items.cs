using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour {

	public List<ItemProperties> itemList;

}

[System.Serializable]
public struct ItemProperties
{
	public string name;

	public GameObject prefab;

	public Sprite icon;

	public float hitDelay;

	public bool stackable;
	public bool holdable;

	public int animalDamage;
	public int treeDamage;
	public int rockDamage;
	public int range;
}