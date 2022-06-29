using System;
using Mirror;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Units
{
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
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

        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawned?.Invoke(this);
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
