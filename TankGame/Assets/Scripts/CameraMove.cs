using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    Vector3 vec;
    [SerializeField] Transform target;

    private void Awake()
    {
        vec = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position + vec;
    }
}
