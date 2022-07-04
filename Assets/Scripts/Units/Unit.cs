using System;
using Combat;
using Mirror;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private Targeter targeter;
        [SerializeField] private Health health;
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned; 
        public static event Action<Unit> ServerOnUnitDespawned;

        public static event Action<Unit> AuthorityOnUnitSpawned; 
        public static event Action<Unit> AuthorityOnUnitDespawned;

        public UnitMovement GetUnitMovement()
        {
            return unitMovement;
        }

        public Targeter GetTargeter()
        {
            return targeter;
        }

        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion

        #region Client

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;
            onDeselected?.Invoke();
        }

        public override void OnStartClient()
        {
            if (!isClientOnly) return;
            if (!hasAuthority) return;
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopClient()
        {
            if (!isClientOnly) return;
            if (!hasAuthority) return;
            AuthorityOnUnitDespawned?.Invoke(this);
        }

        #endregion
    }
}
