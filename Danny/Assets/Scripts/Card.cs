using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
{
    [Header("Visual Settings")]
    [SerializeField] private int normalSortingOrder = 0;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float maxRotationAngle = 45f;

    private Canvas canvas;
    private bool isDragging = false;
    private bool isHighlighted = false;
    private Vector2 offset;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        isHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHighlighted = false;
    }
      
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = true;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPointerPosition))
            {
                offset = GetComponent<RectTransform>().anchoredPosition - localPointerPosition;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            isDragging = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && eventData.button == PointerEventData.InputButton.Left)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out Vector2 localPointerPosition))
            {
                Vector2 newPosition = localPointerPosition + offset;
                GetComponent<RectTransform>().anchoredPosition = newPosition;
            }
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (!isHighlighted) return;

        float scrollDelta = eventData.scrollDelta.y;

        if (IsCtrlPressed())
        {
            canvas.sortingOrder -= (int)Mathf.Sign(scrollDelta);
            Debug.Log($"Новый порядок слоя: {canvas.sortingOrder}");
        }
        else
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            if (currentRotation > 180f) currentRotation -= 360f;

            transform.rotation = Quaternion.Euler(0f, 0f, currentRotation + scrollDelta * rotationSpeed);
        }
    }

    private bool IsCtrlPressed()
    {
        if (Mouse.current == null) return false;

        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            return keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
        }
        return false;
    }
}