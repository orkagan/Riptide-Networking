using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    #region Serialized Fields
    [SerializeField] private float _handlerRange = 1;
    [SerializeField] private float _deadZone = 0;
    [SerializeField] private AxisOptions _axisOptions = AxisOptions.Both;
    [SerializeField] private bool _snapX = false;
    [SerializeField] private bool _snapY = false;
    [SerializeField] protected RectTransform _background = null;
    [SerializeField] protected RectTransform _handle = null;
    public Vector2 input = Vector2.zero;
    #endregion
    #region References
    private RectTransform _baseRect = null;
    private Canvas _canvas = null;
    private Camera _camera = null;
    #endregion
    #region Properties
    public float Horizontal
    {
        get { return (_snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; }
    }
    public float Vertical
    {
        get { return (_snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; }
    }
    public Vector2 Direction
    {
        get { return new Vector2(Horizontal, Vertical); }
    }
    public float HandleRange
    {
        get { return _handlerRange; }
        set { _handlerRange = Mathf.Abs(value); }
    }
    public float DeadZone
    {
        get { return _deadZone; }
        set { _deadZone = Mathf.Abs(value); }
    }
    public AxisOptions AxisOption
    {
        get { return _axisOptions; }
        set { _axisOptions = value; }
    }
    public bool SnapX
    {
        get { return _snapX; }
        set { _snapX = value; }
    }
    public bool SnapY
    {
        get { return _snapY; }
        set { _snapY = value; }
    }
    #endregion
    #region Functions
    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
        {
            return value;
        }
        else if (snapAxis == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle < 67.5f || angle > 112.5f)
                {
                    return 0;
                }
                else
                {
                    return (value > 0) ? 1 : -1;
                }
            }
        }
        else
        {
            if (value > 0)
            {
                return 1;
            }
            if (value < 0)
            {
                return -1;
            }
        }
        return 0;
    }
    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_baseRect, screenPosition, _camera, out localPoint))
        {
            return localPoint - (_background.anchorMax * _baseRect.sizeDelta);
        }
        return Vector2.zero;
    }
    #endregion
    #region Methods
    private void FormatInput()
    {
        if (_axisOptions == AxisOptions.Horizontal)
        {
            input = new Vector2(input.x, 0);
        }
        if (_axisOptions == AxisOptions.Vertical)
        {
            input = new Vector2(0, input.y);
        }
    }
    protected virtual void HandleInput(float magnitude, Vector2 normalized, Vector2 radius, Camera cam)
    {
        if (magnitude > DeadZone)
        {
            if (magnitude > 1)
            {
                input = normalized;
            }
        }
        else
        {
            input = Vector2.zero;
        }
    }
    #endregion
    #region Interfaces
    public void OnDrag(PointerEventData eventData)
    {
        _camera = null;
        if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            _camera = _canvas.worldCamera;
        }
        Vector2 position = RectTransformUtility.WorldToScreenPoint(_camera, _background.position);
        Vector2 radius = _background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * _canvas.scaleFactor);
        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, _camera);
        _handle.anchoredPosition = input * radius * HandleRange;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        _handle.anchoredPosition = Vector2.zero;
    }
    #endregion
    #region Unity Event Functions
    protected virtual void Start()
    {
        HandleRange = _handlerRange;
        DeadZone = _deadZone;
        _baseRect = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Debug.LogError("JOYSTICK NOT INSIDE CANVAS");
        }
        Vector2 center = new Vector2(0.5f, 0.5f);
        _background.pivot = center;
        _handle.anchorMin = center;
        _handle.anchorMax = center;
        _handle.pivot = center;
        _handle.anchoredPosition = Vector2.zero;
    }
    #endregion
}
#region Enums
public enum AxisOptions
{
    Both,
    Horizontal,
    Vertical,
}
#endregion