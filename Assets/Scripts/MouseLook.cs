using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY, MouseX, MouseY }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    public bool invertMouse = false;

    private float _rotationY = 0f;
    private float _rotationX = 0f;

    private void Start()
    {
        var body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
        }
    }

    void Update()
    {
        switch (axes)
        {
            case RotationAxes.MouseX:
                transform.Rotate(0, sensitivityHor * Input.GetAxis("Mouse X"), 0);
                break;
            case RotationAxes.MouseY:
                // Calculate current tilt rotation
                if (invertMouse)
                {
                    _rotationX += Input.GetAxis("Mouse Y") * sensitivityVert;
                }
                else
                {
                    _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
                }
                _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
                // keep current yaw rotation
                _rotationY = transform.localEulerAngles.y;
                // Assign new transform
                transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
                break;
            case RotationAxes.MouseXAndY:
                // Calculate current tilt rotation
                if (invertMouse)
                {
                    _rotationX += Input.GetAxis("Mouse Y") * sensitivityVert;
                }
                else
                {
                    _rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
                }
                _rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);
                // keep current yaw rotation
                var deltaY = Input.GetAxis("Mouse X") * sensitivityHor;
                _rotationY = transform.localEulerAngles.y + deltaY;
                // Assign new transform
                transform.localEulerAngles = new Vector3(_rotationX, _rotationY, 0);
                break;
            default:
                break;
        }
    }
}
