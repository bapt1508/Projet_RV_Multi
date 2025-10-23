/*using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;

public class MoveManager : NetworkBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public float Speed;

    private Transform _previousDest;
    private Transform _currentDest;
    private float _timeToWaypoint;
    private float _elapsedTime;
    private Transform _destination;
    public void Start()
    {
        
        TargetWayPoint(StartPoint,EndPoint); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        _elapsedTime += Time.deltaTime;
        float elapsedPercentage = _elapsedTime/_timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previousDest.position, _currentDest.position, elapsedPercentage);
        
        if (elapsedPercentage >= 1)
        {
            TargetWayPoint(_currentDest,_previousDest);
        }
    }
    private void TargetWayPoint(Transform DebutPoint,Transform Finpoint)
    {
        Debug.Log("Switching");
        _previousDest= DebutPoint;
        _currentDest= Finpoint;
        _elapsedTime = 0;
        float Distance = Vector3.Distance(_previousDest.position, _currentDest.position);
        _timeToWaypoint = Distance / Speed;
        Debug.Log(_timeToWaypoint);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        other.transform.SetParent(transform);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        other.transform.SetParent(null);
    }


}*/

using UnityEngine;
using Unity.Netcode;

public class MoveManager : NetworkBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public float Speed = 2;

    private Transform _previousDest;
    private Transform _currentDest;
    private float _timeToWaypoint;
    private float _elapsedTime;

    private Vector3 _lastPos;
    public Vector3 PlatformVelocity { get; private set; }

    void Start()
    {
        if (IsServer)
            TargetWayPoint(StartPoint, EndPoint);

        _lastPos = transform.position;
    }

    /*void FixedUpdate()
    {
        if (!IsServer) return;

        // Compute motion
        _elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.SmoothStep(0, 1, _elapsedTime / _timeToWaypoint);
        transform.position = Vector3.Lerp(_previousDest.position, _currentDest.position, t);

        // Compute velocity for passengers
        PlatformVelocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        _lastPos = transform.position;

        if (t >= 1f)
            TargetWayPoint(_currentDest, _previousDest);
    }*/
    void FixedUpdate()
    {
        // === CALCUL VELOCITY (PARTOUT) ===
        Vector3 currentPos = transform.position;
        PlatformVelocity = (currentPos - _lastPos) / Time.fixedDeltaTime;
        _lastPos = currentPos;


        // === MOUVEMENT SEULEMENT SERVER ===
        if (!IsServer) return;

        _elapsedTime += Time.fixedDeltaTime;
        float t =  _elapsedTime / _timeToWaypoint;
        transform.position = Vector3.Lerp(_previousDest.position, _currentDest.position, t);

        if (t >= 1f)
            TargetWayPoint(_currentDest, _previousDest);
    }


    private void TargetWayPoint(Transform a, Transform b)
    {
        _previousDest = a;
        _currentDest = b;
        _elapsedTime = 0;
        float dist = Vector3.Distance(a.position, b.position);
        _timeToWaypoint = dist / Speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (!IsOwner) return;
        var p = other.GetComponent<PlayerOnPlatform>();
        if (p) p.SetPlatform(this);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (!IsOwner) return;
        
        var p = other.GetComponent<PlayerOnPlatform>();
        if (p) p.ClearPlatform(this);
    }
}

