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
        
        UpdateWorldAimPosition();
    }
    
    public void SetAimBounds(Bounds bounds)
    {
        aimBounds = bounds;
        hasBounds = true;
        
        CenterCursorOnBounds();
    }
    
    private void CenterCursorOnBounds()
    {
        if (!hasBounds || playerCamera == null) return;
        
        Vector3 screenPos = playerCamera.WorldToScreenPoint(aimBounds.center);
        
        if (screenPos.z > 0)
        {
            screenCursor = new Vector2(screenPos.x, screenPos.y);
            
            UpdateWorldAimPosition();
        }
    }

    public void OnAim(InputAction.CallbackContext context) => aimInput = context.ReadValue<Vector2>();
    public void OnPressedInput(InputAction.CallbackContext context) => OnClicked?.Invoke();
    public void OnExitInput(InputAction.CallbackContext context) => OnExit?.Invoke();
    public void OnSelectInput(InputAction.CallbackContext context) => OnSelectBuild?.Invoke(10);
    
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    
    void Update()
    {
        screenCursor += aimInput * cursorSpeed * Time.deltaTime;

        screenCursor.x = Mathf.Clamp(screenCursor.x, 0, Screen.width);
        screenCursor.y = Mathf.Clamp(screenCursor.y, 0, Screen.height);

        UpdateWorldAimPosition();
    }
    
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