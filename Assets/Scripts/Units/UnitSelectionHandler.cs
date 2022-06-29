using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        private Camera mainCamera;
        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        [SerializeField] private LayerMask layerMask;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                //Start the selection area.
                StartSelectionArea();
            }

            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                //Clear the selection area.
                ClearSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            SelectedUnits.Clear();
        }
    
        private void ClearSelectionArea()
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;
            if (!hit.collider.TryGetComponent(out Unit unit)) return;
            if (!unit.hasAuthority) return;
        
            SelectedUnits.Add(unit);

            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }
        }
    }
}
