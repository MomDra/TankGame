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

    [SerializeField] Transform turret;

    Rigidbody rigid;
    Ray ray;
    RaycastHit hit;
    Camera mainCamera;

    [SerializeField] GameObject prefab;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");

        RotateTurret();
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

    void RotateTurret()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 pos = new Vector3(hit.point.x, turret.position.y, hit.point.z);

            Vector3 directionToFace = pos - turret.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToFace);
            turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
                Instantiate(prefab, hit.point, Quaternion.identity);
        }
    }
}
