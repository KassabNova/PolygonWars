using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsShootable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    void Start()
    {
        camera = GameObject.Find("FirstPersonCharacter").GetComponent<Transform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartShooting();
        }
        //else if (Input.GetMouseButtonUp(0))
        //{
        //    StopGrapple();
        //}
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartShooting()
    {
        ShootBullet();

        RaycastHit hit;
        bool test = Physics.Raycast(camera.position, camera.forward, out hit, maxDistance, whatIsShootable);
        if (test)
        {
            Debug.Log($"It shoot the UUI!!! {hit.point}");
        }
        float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

        currentGrapplePosition = gunTip.position;
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopShooting()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void ShootBullet()
    {
        //If not grappling, don't draw rope

    }

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
