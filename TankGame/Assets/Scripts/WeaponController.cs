using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] GameObject bomb;
    [SerializeField] Transform endOfGun;
    [SerializeField] Transform gun;
    [SerializeField] int power;
    [SerializeField] float fireDelay;
    [SerializeField] Transform turret;
    [SerializeField] Transform attackRange;
    [SerializeField] float time;

    Vector3 vo;

    int lineSegment;
    LineRenderer lineVisual;

    Rigidbody bombRigid;

    Ray ray;
    RaycastHit hit;
    Plane turretPlane;
    Plane groundPlane;
    Camera mainCamera;

    [SerializeField] GameObject cube;

    private void Awake()
    {
        mainCamera = Camera.main;
        turretPlane = new Plane(Vector3.up, turret.position);
        groundPlane = new Plane(Vector3.up, Vector3.zero);

        float tmp = CalLength(45) * 2;
        attackRange.localScale = new Vector3(tmp, tmp, 0f);

        lineVisual = GetComponent<LineRenderer>();
        lineSegment = lineVisual.positionCount;
    }

    private void Update()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            vo = CalVelocity(hit.point, endOfGun.position, power);
            //Visualize(vo);


            // 회전 관련;;
           
            if (Input.GetMouseButtonDown(0))
            {
                GameObject clone = Instantiate(bomb, endOfGun.position, endOfGun.rotation);
                bombRigid = clone.GetComponent<Rigidbody>();
                //rigid.AddForce(endOfGun.transform.forward.normalized * power, ForceMode.Impulse);
                Debug.Log(vo.magnitude);
                bombRigid.velocity = vo;
            }
        }
    }

    private void FixedUpdate()
    {
        RotateTurret();
        TurretUpdownRotate();
    }

    void RotateTurret()
    {
        // 인터넷 검색해서 찾은 방법
        float L;

        if(turretPlane.Raycast(ray, out L))
        {
            Vector3 pointToLook = ray.GetPoint(L);

            turret.LookAt(pointToLook);

            /*
            Vector3 directionToFace = pointToLook - turret.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToFace);
            turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime);*/
        }

        /* 내각 생각한 방법
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Vector3 pos = new Vector3(hit.point.x, turret.position.y, hit.point.z);

            Vector3 directionToFace = pos - turret.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToFace);
            turret.rotation = Quaternion.Slerp(turret.rotation, targetRotation, Time.deltaTime);

            
            if (Input.GetMouseButtonDown(0))
                Instantiate(prefab, hit.point, Quaternion.identity);
        }*/
    }

    float CalLength(float deg)
    {
        return (power * power * Mathf.Sin(deg)) / 9.8f;
    }

    Vector3 CalVelocity2(Vector3 target, Vector3 origin, float time)
    {
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float sY = distance.y;
        float sXZ = distanceXZ.magnitude;

        float vXZ = sXZ / time;
        float vY = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

        Vector3 result = distanceXZ.normalized;
        result *= vXZ;
        result.y = vY;

        return result;
    }

    Vector3 CalVelocity(Vector3 target, Vector3 origin, float scalaVelocity)
    {
        // 포물선 x 변위, y 변위, 초기 속력 고정 으로 시간 t를 구하는 방정식

        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0f;

        float sXZ = distanceXZ.magnitude;
        float sY = distance.y;

        float A = Mathf.Pow(Mathf.Abs(Physics.gravity.y) / 2, 2);
        float B = Physics.gravity.y * sY - Mathf.Pow(scalaVelocity, 2);
        float C = Mathf.Pow(sXZ, 2) + Mathf.Pow(sY, 2);

        float D = Mathf.Pow(B, 2) - 4 * A * C;

        Vector3 errorResult = distanceXZ.normalized;
        errorResult.y = 1f;
        errorResult = errorResult.normalized * scalaVelocity;

        if (D < 0)
        {
            Debug.Log("판별식 에서 에러");
            
            return errorResult;
        }

        float R1 = (-B + Mathf.Sqrt(D)) / (2 * A);
        float R2 = (-B - Mathf.Sqrt(D)) / (2 * A);

        float T = Mathf.NegativeInfinity;
        
        if (R1 > 0 && R2 > 0)
        {
            T = R1 < R2 ? R1 : R2;
        }
        else if(R1 <= 0 && R2 <= 0)
        { // 해가 없을 때 RR > 0 이어야 함
            Debug.Log("방정식 구한 해에서 에러");

            return errorResult;
        }
        else
        {
            T = R1 > R2 ? R1 : R2;
        }

        T = Mathf.Sqrt(T);

        return CalVelocity2(target, origin, T);
    }

    Vector3 CalPosInTime(Vector3 vo, float time)
    {
        Vector3 vXZ = vo;
        vXZ.y = 0f;

        Vector3 result = new Vector3(endOfGun.position.x, 0f, endOfGun.position.z) + vXZ * time;
        result.y = endOfGun.position.y + (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time);

        return result;
    }

    void TurretUpdownRotate()
    {
        Vector3 _vo = vo;
        float vy = _vo.y;
        _vo.y = 0;
        float vx = _vo.magnitude;

        float deg = -Mathf.Rad2Deg * Mathf.Atan2(vy, vx);

        Debug.Log(deg);

        gun.localRotation = Quaternion.AngleAxis(deg, Vector3.right);
    }

    void Visualize()
    {
        for (int i = 0; i < lineSegment; i++)
        {
            Vector3 pos = CalPosInTime(vo, i / (float)lineSegment);
            lineVisual.SetPosition(i, pos);
        }
    }
}
