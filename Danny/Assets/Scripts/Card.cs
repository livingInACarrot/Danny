using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler, IPointerClickHandler
{
    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 0.5f;

    public bool InHand = false;
    public float Width => GetComponent<RectTransform>().rect.width;
    public float Height => GetComponent<RectTransform>().rect.height;

    private Canvas canvas;
    private PlayingCardsTable cardsHandler;
    private bool isDragging = false;
    //private bool isHighlighted = false;
    private Vector2 offset;

    public void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        cardsHandler = canvas.gameObject.GetComponentInParent<PlayingCardsTable>();

        if (canvas == null)
            Debug.Log("Null canv");
        if (cardsHandler == null)
            Debug.Log("Null cardsHandler");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Pointer Enter");
        //isHighlighted = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exit");
        //isHighlighted = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !InHand)
        {
            cardsHandler.ReturnCardToHand(this);
        }
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
            if (InHand) cardsHandler.PlaceCardFromHandOnTable(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && eventData.button == PointerEventData.InputButton.Left)
        {
            FollowPointer(eventData);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        //if (!isHighlighted) return;

        float scrollDelta = eventData.scrollDelta.y;

        if (Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed)
        {
            cardsHandler.ChangeCardInOrder(this, -(int)Mathf.Sign(scrollDelta));
            Debug.Log($"Новый порядок слоя: {canvas.sortingOrder}");
        }
        else
        {
            RotateCard(scrollDelta);
        }
    }

    public void ChangeLayer(int newLayer)
    {
        canvas.sortingOrder = newLayer;
    }

    public void ReturnToHand(Vector2 newPos)
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = newPos;
        rect.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Private methods-helpers
    private void RotateCard(float delta)
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        if (currentRotation > 180f) currentRotation -= 360f;
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation + delta * rotationSpeed);
    }

    private void FollowPointer(PointerEventData eventData)
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