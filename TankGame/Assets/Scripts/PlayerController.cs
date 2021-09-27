using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;
    [SerializeField] float turretTurnSpeed;

    float verticalInput;
    float horizontalInput;
    float mouseX;
    float mouseY;

    

    Rigidbody rigid;
    Ray ray;
    RaycastHit hit;
    Camera mainCamera;

    [SerializeField] GameObject prefab;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (Input.GetButton("Vertical"))
        {
            MoveForward();
        }

        if (Input.GetButton("Horizontal"))
        {
            RotateTank();
        }
    }

    void MoveForward()
    {
        Vector3 moveVertical = transform.forward * verticalInput;
        Vector3 dirVec = moveVertical * moveSpeed * Time.deltaTime;

        rigid.MovePosition(rigid.position + dirVec);
    }

    void RotateTank()
    {
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
    }
}
