using System;
using Combat;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damageToDeal = 20;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }

        if (other.TryGetComponent(out Health health))
        {
            health.DealDamage(damageToDeal);
        }
        
        DestroySelf();
    }
}
