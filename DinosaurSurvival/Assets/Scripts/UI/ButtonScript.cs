using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{

    public GameObject generationCanvas;
    GameObject inventoryObject;

    void Awake()
    {
        inventoryObject = GameObject.Find("Inventory");
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == "scene_menu")
        {
            generationCanvas.SetActive(true);
            GameObject.Find("MenuCanvas").SetActive(false);

            SceneManager.LoadScene("scene_main");
        }
    }

    public void InventoryButton()
    {
        inventoryObject.GetComponent<Inventory>().ItemClick(gameObject);
    }
}
