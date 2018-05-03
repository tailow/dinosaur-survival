﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    #region Variables

    public int walkSpeed;
    public int sprintSpeed;

    float currentSpeed;
    float targetSpeed;
    float t;

    int jumpHeight = 3;
    int sensitivity = 3;

    public float movementAcceleration;
    public float stopAcceleration;

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
        targetSpeed = walkSpeed;

        if (Input.GetButton("Sprint") && Input.GetAxisRaw("Vertical") > 0)
        {
            targetSpeed = sprintSpeed;
        }

        if (Input.GetButtonUp("Sprint"))
        {
            targetSpeed = walkSpeed;
        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            t = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, t += Time.deltaTime * movementAcceleration);

            if (Mathf.Abs(currentSpeed - targetSpeed) < 0.01f)
            {
                currentSpeed = targetSpeed;
            }

            dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        else
        {
            t = 0f;

            targetSpeed = 0f;

            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, t += Time.deltaTime * stopAcceleration);

            if (Mathf.Abs(currentSpeed - targetSpeed) < 0.01f)
            {
                currentSpeed = targetSpeed;
            }
        }

        movement = dir.normalized * currentSpeed;

        rigid.MovePosition(rigid.position + transform.TransformDirection(movement) * Time.deltaTime);


        // JUMPING
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rigid.AddForce(Vector3.up * jumpHeight * 2, ForceMode.Impulse);
        }
    }

    void Update()
    {
        // MOUSE INPUT
        transform.Rotate(new Vector3(0, Input.GetAxisRaw("Mouse X") * sensitivity, 0));

        Camera.main.transform.Rotate(new Vector3(Input.GetAxisRaw("Mouse Y") * -sensitivity, 0, 0));
    }

    bool IsGrounded()
    {
        if (Physics.Raycast(rigid.position, Vector3.down, transform.localScale.y + 0.2f))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
