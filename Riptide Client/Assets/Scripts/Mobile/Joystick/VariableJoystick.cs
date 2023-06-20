using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VariableJoystick : Joystick
{
    #region Variables
    [SerializeField] private JoystickType _joystickType = JoystickType.Fixed;
    [SerializeField] private float _moveThreshold = 1;
    private Vector2 _fixedPosition = Vector2.zero;
    #endregion
    #region Properties
    public float MoveThreshold
    {
        get { return _moveThreshold; }
        set { _moveThreshold = Mathf.Abs(value); }
    }
    #endregion
    #region Methods
    public void SetMode(JoystickType joystickType)
    {
        _joystickType = joystickType;
        if (joystickType == JoystickType.Fixed)
        {
            _background.anchoredPosition = _fixedPosition;
            _background.gameObject.SetActive(true);
        }
        else
        {
            _background.gameObject.SetActive(false);
        }
    }

    protected override void HandleInput(float magnitude, Vector2 normalized, Vector2 radius, Camera cam)
    {
        if (_joystickType == JoystickType.Dynamic && magnitude > MoveThreshold)
        {
            Vector2 difference = normalized * (magnitude - MoveThreshold) * radius;
            _background.anchoredPosition += difference;
        }
        base.HandleInput(magnitude, normalized, radius, cam);
    }
    #endregion
    #region Interfaces
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (_joystickType != JoystickType.Fixed)
        {
            _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            _background.gameObject.SetActive(true);
        }
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (_joystickType != JoystickType.Fixed)
        {
            _background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            _background.gameObject.SetActive(false);
        }
        base.OnPointerUp(eventData);
    }
    #endregion
    #region Unity Event Functions
    protected override void Start()
    {
        base.Start();
        _fixedPosition = _background.anchoredPosition;
        SetMode(_joystickType);
    }
    #endregion
}
public enum JoystickType
{
    Fixed, //the static
    Floating, //static while screen touch
    Dynamic, //follow touch
}