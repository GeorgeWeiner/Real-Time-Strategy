using System;
using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {

        private Vector2 startPosition;
        private Camera _mainCamera;
        private RTSPlayer _player;
        public List<Unit> SelectedUnits { get; } = new List<Unit>();

        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private LayerMask layerMask;

        private void Start()
        {
            _mainCamera = Camera.main;

            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        

        private void Update()
        {
            if (_player == null)
            {
                _player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }

            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }
        
        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var selectedUnit in SelectedUnits)
                {
                    selectedUnit.Deselect();
                }

                SelectedUnits.Clear();
            }
            
            unitSelectionArea.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            
            UpdateSelectionArea();
        }
    
        private void ClearSelectionArea()
        {
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask)) return;
                if (!hit.collider.TryGetComponent(out Unit unit)) return;
                if (!unit.hasAuthority) return;
        
                SelectedUnits.Add(unit);

                foreach (var selectedUnit in SelectedUnits)
                {
                    selectedUnit.Select();
                }
                return;
            }

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

            foreach (var unit in _player.GetMyUnits())
            {
                // < >

                if (SelectedUnits.Contains(unit)) continue;
                
                Vector3 screenPosition = _mainCamera.WorldToScreenPoint(unit.transform.position);
                if (screenPosition.x > min.x && screenPosition.y > min.y &&
                    screenPosition.x < max.x && screenPosition.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }
        
        private void UpdateSelectionArea()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            float areaWidth = mousePosition.x - startPosition.x;
            float areaHeight = mousePosition.y - startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
            unitSelectionArea.anchoredPosition =
                startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
        }
        
        private void AuthorityHandleUnitDespawned(Unit unit)
        {
            SelectedUnits.Remove(unit);
        }
    }
}
