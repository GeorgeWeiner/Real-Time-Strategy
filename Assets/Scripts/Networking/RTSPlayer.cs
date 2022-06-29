using System.Collections.Generic;
using Mirror;
using Units;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
   [SerializeField]
   private List<Unit> myUnits = new List<Unit>();

   public override void OnStartServer()
   {
      Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
      Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
   }

   public override void OnStopServer()
   {
      Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
      Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
   }

   private void ServerHandleUnitSpawned(Unit unit)
   {
      if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
      
      myUnits.Add(unit);
   }
   
   private void ServerHandleUnitDespawned(Unit unit)
   {
      if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
      
      myUnits.Remove(unit);
   }

   public override void OnStartClient()
   {
      if (!hasAuthority) return;
      
      Unit.AuthorityOnUnitSpawned += ServerHandleUnitSpawned;
      Unit.AuthorityOnUnitDespawned += ServerHandleUnitDespawned;
   }

   public override void OnStopClient()
   {
      if (!hasAuthority) return;

      Unit.AuthorityOnUnitSpawned -= ServerHandleUnitSpawned;
      Unit.AuthorityOnUnitDespawned -= ServerHandleUnitDespawned;
   }

   private void AuthorityHandleUnitSpawned(Unit unit)
   {
      if (!hasAuthority) return;
      myUnits.Add(unit);
   }
   
   private void AuthorityHandleUnitDespawned(Unit unit)
   {
      if (!hasAuthority) return;
      myUnits.Remove(unit);
   }
}
