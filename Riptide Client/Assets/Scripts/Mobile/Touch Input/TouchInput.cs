using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    public enum ConnectionType
    {
        Mobile,
        Windows,
        Both,
    }
    public ConnectionType connectionType = ConnectionType.Mobile;
    
    //the layer we can interact with
    public LayerMask touchLayer;
    //current touches
    private List<GameObject> _touchList = new List<GameObject>();
    //old touches
    private GameObject[] _oldTouches;
    //touch info
    private RaycastHit _hit;
    void Update()
    {
        if (connectionType == ConnectionType.Mobile)
        {
            Mobile();
        }
        else if (connectionType == ConnectionType.Windows)
        {
            Windows();
        }
        else
        {
            Mobile();
            Windows();
        }
    }
    void Mobile()
    {
        if (Input.touchCount > 0)
        {
            _oldTouches = new GameObject[_touchList.Count];
            _touchList.CopyTo(_oldTouches);
            _touchList.Clear();

            foreach (Touch touch in Input.touches)
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out _hit, touchLayer))
                {
                    GameObject recipient = _hit.transform.gameObject;
                    _touchList.Add(recipient);
                    if (touch.phase == TouchPhase.Began)
                    {
                        recipient.SendMessage("OnTouchDown", _hit.point, SendMessageOptions.DontRequireReceiver);
                        Debug.Log("OnTouchDown");
                    }
                    if (touch.phase == TouchPhase.Stationary)
                    {
                        recipient.SendMessage("OnTouchStay", _hit.point, SendMessageOptions.DontRequireReceiver);
                        Debug.Log("OnTouchStay");
                    }
                    if (touch.phase == TouchPhase.Ended)
                    {
                        recipient.SendMessage("OnTouchUp", _hit.point, SendMessageOptions.DontRequireReceiver);
                        Debug.Log("OnTouchUp");
                    }
                    if (touch.phase == TouchPhase.Canceled)
                    {
                        recipient.SendMessage("OnTouchExit", _hit.point, SendMessageOptions.DontRequireReceiver);
                        Debug.Log("OnTouchExit");
                    }
                }
                foreach (GameObject item in _oldTouches)
                {
                    if (!_touchList.Contains(item))
                    {
                        item.SendMessage("OnTouchExit", _hit.point, SendMessageOptions.DontRequireReceiver);
                        Debug.Log("OnTouchExit");
                    }
                }
            }
        }
    }
    void Windows()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            _oldTouches = new GameObject[_touchList.Count];
            _touchList.CopyTo(_oldTouches);
            _touchList.Clear();

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            //if this ray hits something on the touch layer
            if (Physics.Raycast(ray, out _hit, touchLayer))
            {
                GameObject recipient = _hit.transform.gameObject;
                _touchList.Add(recipient);
                if (Input.GetMouseButtonDown(0))
                {
                    recipient.SendMessage("OnTouchDown", _hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("OnTouchDown");
                }
                if (Input.GetMouseButton(0))
                {
                    recipient.SendMessage("OnTouchStay", _hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("OnTouchStay");
                }
                if (Input.GetMouseButtonUp(0))
                {
                    recipient.SendMessage("OnTouchUp", _hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("OnTouchUp");
                }
            }
            foreach(GameObject item in _oldTouches)
            {
                if (!_touchList.Contains(item))
                {
                    item.SendMessage("OnTouchExit", _hit.point, SendMessageOptions.DontRequireReceiver);
                    Debug.Log("OnTouchExit");
                }
            }
        }
    }
}
