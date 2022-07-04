using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Units
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform unitSpawnPoint;
        [SerializeField] private Health health;

        #region Server

        [Command]
        private void CmdSpawnUnit()
        {
            var unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);
            NetworkServer.Spawn(unitInstance, connectionToClient);
        }

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        [Server]
        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }

        #endregion
    
        #region Client 
    
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
        
            CmdSpawnUnit();
        }
    
        #endregion

    
    }
}
