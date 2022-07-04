using Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitCommandGiver : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask;

        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity)) return;
            if (hit.collider.TryGetComponent(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point);
                    return;
                }

                TryTarget(target);
                return;
            }

            TryMove(hit.point);
        }

        /// <summary>
        /// Set a new target for all the selected units.
        /// </summary>
        /// <param name="target"></param>
        private void TryTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetTargeter().CmdSetTarget(target.gameObject);
            }
        }

        /// <summary>
        /// Set a new movement destination for all the selected units.
        /// </summary>
        /// <param name="point"></param>
        private void TryMove(Vector3 point)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }
    }
}
