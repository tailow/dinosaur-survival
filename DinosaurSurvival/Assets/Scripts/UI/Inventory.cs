using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{

    #region Variables

    public int inventorySize;

    bool isHoldingItem;

    GameObject holdedItem;

    public Items items;
    public Transform hotbar;

    public InventoryItem[] inventoryItems;

    #endregion

    void Start()
    {
        inventoryItems = new InventoryItem[inventorySize + hotbar.childCount];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isHoldingItem && gameObject.activeInHierarchy == true && holdedItem != null)
        {
            StopAllCoroutines();

            holdedItem.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 1;

            holdedItem.transform.GetChild(0).localPosition = Vector3.zero;

            isHoldingItem = false;
        }
    }

    public void ItemClick(GameObject item)
    {
        if (!isHoldingItem)
        {
            isHoldingItem = true;

            holdedItem = item;

            item.transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 3;

            StartCoroutine(ItemHoldCoroutine(item));
        }
    }

    IEnumerator ItemHoldCoroutine(GameObject item)
    {
        while (true)
        {
            item.transform.GetChild(0).position = Input.mousePosition;

            yield return new WaitForEndOfFrame();
        }
    }

    public void AddItem(int itemId, int amount)
    {

    }

    ItemProperties GetItem(string name)
    {
        for (int i = 0; i < items.itemList.Count; i++)
        {
            if (items.itemList[i].name == name)
            {
                return items.itemList[i];
            }
        }

        return items.itemList[0];
    }
}

[System.Serializable]
public struct InventoryItem
{
    public string itemName;
    public int itemAmount;
}