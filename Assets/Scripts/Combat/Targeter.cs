using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;
    
        [Command] 
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent(out Targetable target)) return;
            this.target = target;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }

        public Targetable GetTarget()
        {
            return target;
        }
    }
}
