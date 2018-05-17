using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMechanics : MonoBehaviour
{

    #region Variables

    Transform hotbar;

    public Items items;
    public GameObject inventory;

    public Transform hotbarSlotParent;

    int currentSlot;
    int nextSlot;

    #endregion

    void Start()
    {
        hotbar = Camera.main.transform.GetChild(0);

        currentSlot = 1;
        nextSlot = 1;

        SwapItem();

        int index = 1;

        foreach (Transform child in hotbarSlotParent)
        {
            child.GetChild(0).GetChild(0).GetComponent<Image>().sprite = GetItem(CheckItem(index).name).icon;

            index++;
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && CheckItem(currentSlot).GetComponent<Animator>() != null && !inventory.activeInHierarchy)
        {
            if (!CheckItem(currentSlot).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Swing"))
            {
                CheckItem(currentSlot).GetComponent<Animator>().Play("Swing");

                Invoke("Swing", GetItem(CheckItem(currentSlot).name).hitDelay);
            }
        }

        #region Hotbar input

        if (Input.GetKeyDown(KeyCode.Alpha1) && GetItem(CheckItem(1).name).holdable)
        {
            nextSlot = 1;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && GetItem(CheckItem(2).name).holdable)
        {
            nextSlot = 2;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && GetItem(CheckItem(3).name).holdable)
        {
            nextSlot = 3;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4) && GetItem(CheckItem(4).name).holdable)
        {
            nextSlot = 4;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5) && GetItem(CheckItem(5).name).holdable)
        {
            nextSlot = 5;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha6) && GetItem(CheckItem(6).name).holdable)
        {
            nextSlot = 6;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha7) && GetItem(CheckItem(7).name).holdable)
        {
            nextSlot = 7;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha8) && GetItem(CheckItem(8).name).holdable)
        {
            nextSlot = 8;

            SwapItem();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9) && GetItem(CheckItem(9).name).holdable)
        {
            nextSlot = 9;

            SwapItem();
        }
        #endregion

        if (Input.GetButtonDown("Inventory"))
        {
            inventory.SetActive(!inventory.activeInHierarchy);

            if (inventory.activeInHierarchy)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void Swing()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position,
        new Vector3(0.3f, 0.3f, GetItem(CheckItem(currentSlot).name).range), Camera.main.transform.rotation);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag == "Animal")
            {
                hitColliders[i].GetComponent<AnimalBehaviour>().TakeDamage(GetItem(CheckItem(currentSlot).name).animalDamage);
            }

            if (hitColliders[i].tag == "Tree")
            {
                hitColliders[i].GetComponent<CollectableItemBehaviour>().TakeDamage(GetItem(CheckItem(currentSlot).name).treeDamage);
            }

            if (hitColliders[i].tag == "Rock")
            {
                hitColliders[i].GetComponent<CollectableItemBehaviour>().TakeDamage(GetItem(CheckItem(currentSlot).name).rockDamage);
            }
        }
    }

    void SwapItem()
    {
        CheckItem(currentSlot).SetActive(false);

        hotbarSlotParent.GetChild(currentSlot - 1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        hotbarSlotParent.GetChild(nextSlot - 1).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1f);

        CheckItem(nextSlot).SetActive(true);

        currentSlot = nextSlot;
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

    GameObject CheckItem(int slot)
    {
        return hotbar.GetChild(slot - 1).GetChild(0).gameObject;
    }
}
