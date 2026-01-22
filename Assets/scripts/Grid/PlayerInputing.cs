using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerInputing : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;

    [Header("Aim settings")]
    public float cursorSpeed = 1200f;
    public float maxDistance = 100f;
    public LayerMask hitMask;

    public Vector2 aimInput;
    Vector2 screenCursor;
    Vector3 worldAimPosition;

    public Action OnClicked, OnExit;
    public Action<int> OnSelectTroop, OnSelectBuild;

    public bool isPlayerOne;
    
    private Bounds aimBounds;
    private bool hasBounds = false;

    public bool IsReady = false;
    
    void Start()
    {
        screenCursor = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        // Initialiser worldAimPosition au centre de l'écran
        UpdateWorldAimPosition();
    }
    
    public void SetAimBounds(Bounds bounds)
    {
        aimBounds = bounds;
        hasBounds = true;
        
        // Centrer le curseur sur la grid
        CenterCursorOnBounds();
    }
    
    // Nouvelle méthode pour centrer le curseur sur les bounds
    private void CenterCursorOnBounds()
    {
        if (!hasBounds || playerCamera == null) return;
        
        // Convertir le centre des bounds en position écran
        Vector3 screenPos = playerCamera.WorldToScreenPoint(aimBounds.center);
        
        // Si le point est visible à l'écran
        if (screenPos.z > 0)
        {
            screenCursor = new Vector2(screenPos.x, screenPos.y);
            
            // Mettre à jour immédiatement la position world
            UpdateWorldAimPosition();
        }
    }

    public void OnAim(InputAction.CallbackContext context) => aimInput = context.ReadValue<Vector2>();
    public void OnPressedInput(InputAction.CallbackContext context) => OnClicked?.Invoke();
    public void OnExitInput(InputAction.CallbackContext context) => OnExit?.Invoke();
    
    public void OnXPress(InputAction.CallbackContext context) => OnSelectTroop?.Invoke(2);
    public void OnYPress(InputAction.CallbackContext context) => OnSelectTroop?.Invoke(27);
    
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    
    void Update()
    {
        screenCursor += aimInput * cursorSpeed * Time.deltaTime;

        screenCursor.x = Mathf.Clamp(screenCursor.x, 0, Screen.width);
        screenCursor.y = Mathf.Clamp(screenCursor.y, 0, Screen.height);

        UpdateWorldAimPosition();
    }
    
    // Méthode séparée pour mettre à jour la position world
    private void UpdateWorldAimPosition()
    {
        if (playerCamera == null) return;
        
        Ray ray = playerCamera.ScreenPointToRay(screenCursor);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            if (hasBounds)
            {
                hitPoint.x = Mathf.Clamp(hitPoint.x, aimBounds.min.x, aimBounds.max.x);
                hitPoint.z = Mathf.Clamp(hitPoint.z, aimBounds.min.z, aimBounds.max.z);
            }

            worldAimPosition = hitPoint;

            Debug.DrawLine(ray.origin, worldAimPosition, Color.green);
        }
    }

    public Vector3 GetWorldAimPosition()
    {
        return new Vector3(worldAimPosition.x, 0f, worldAimPosition.z);
    }
    
    void OnDrawGizmos()
    {
        if (!hasBounds) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(aimBounds.center, aimBounds.size);
    }
}