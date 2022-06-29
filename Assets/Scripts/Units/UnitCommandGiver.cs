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

            TryMove(hit.point);
        }

        private void TryMove(Vector3 point)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.GetUnitMovement().CmdMove(point);
            }
        }
    }
}
