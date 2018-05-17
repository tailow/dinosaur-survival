using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables

    public GameObject player;
    public GameObject inventory;

    #endregion

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inventory.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Application.Quit();
        }
    }
}
