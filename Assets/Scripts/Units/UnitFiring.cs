using System;
using System.Collections;
using System.Collections.Generic;
using Combat;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        
        if (target == null) return;
        if (!CanFireAtTarget()) return;

        Quaternion targetRotation =
            Quaternion.LookRotation(targeter.GetTarget().transform.position - transform.position);
        
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = 
                Quaternion.LookRotation(targeter.GetTarget().GetAimAtPoint().position - projectileSpawnPoint.position);
            
            var projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);
            
            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }
    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude 
               <= Mathf.Pow(fireRange, 2);
    }
}
