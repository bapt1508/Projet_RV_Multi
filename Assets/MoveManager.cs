using System.Collections;
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


}
