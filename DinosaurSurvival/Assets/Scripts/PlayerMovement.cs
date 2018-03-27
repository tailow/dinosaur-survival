using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variables

    int movementSpeed = 10;
    int jumpHeight = 3;
    int sensitivity = 3;

    Vector3 dir;
    Vector3 movement;

    Rigidbody rigid;

    #endregion

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // MOVEMENT
        dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        movement = dir.normalized * movementSpeed;

        rigid.MovePosition(rigid.position + transform.TransformDirection(movement) * Time.deltaTime);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0));

        Camera.main.transform.Rotate(new Vector3(Input.GetAxisRaw("Mouse Y") * -sensitivity, 0, 0));
    }

}
