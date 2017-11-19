using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Button tap feedback
// TODO: Menu tagalong...

public class NavigationAction : MonoBehaviour {
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 10.0f;
    public float MoveSensitivity = 2f;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.X;

    private int nextAxis = 0;
    private Vector3 manipulationPreviousPosition;

    void Update()
    {
        if (NavigationManager.Instance.IsNavigating && HandsManager.Instance.FocusedGameObject == gameObject)
        {
            PerformRotation();
        }
    }

    private void PerformRotation()
    {
        var rotationFactor = NavigationManager.Instance.NavigationSpeed * RotationSensitivity;
        //Debug.Log(string.Format("{0}",rotationFactor));

        switch (rotationAxis)
        {
            case RotationAxis.X:
                HUDText.Instance.axislabel = string.Format("{0} {1} {2} deg", gameObject.name, "X", rotationFactor);
                transform.Rotate(rotationFactor, 0, 0);
                break;
            case RotationAxis.Y:
                HUDText.Instance.axislabel = string.Format("{0} {1} {2} deg", gameObject.name, "Y", rotationFactor);
                transform.Rotate(0, rotationFactor, 0);
                break;
            case RotationAxis.Z:
                HUDText.Instance.axislabel = string.Format("{0} {1} {2} deg", gameObject.name, "Z", rotationFactor);
                transform.Rotate(0, 0, rotationFactor);
                break;
        }
    }

    void PerformManipulationStart(Vector3 position)
    {
        if (HandsManager.Instance.FocusedGameObject == gameObject)
        {
            Debug.Log("Manipulation Start");
            HUDText.Instance.axislabel = string.Format("{0} Move", gameObject.name);

            manipulationPreviousPosition = position;
        }
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (HandsManager.Instance.FocusedGameObject == gameObject)
        {
            Debug.Log("Manipulation Update");
            if (NavigationManager.Instance.IsManipulating)
            {
                Vector3 moveVector = Vector3.zero;
                moveVector = position - manipulationPreviousPosition;
                manipulationPreviousPosition = position;
                transform.position += (moveVector * MoveSensitivity);
            }
        }
    }

    void OnSelect()
    {
        Debug.Log("OnSelect for " + gameObject.name);

        if (GazeManager.Instance.FocusedGameObject == gameObject)
        {
            nextAxis++;
            nextAxis = nextAxis % 3;
            Debug.Log("Next axis = " + nextAxis);
            switch (nextAxis)
            {
                case 0: rotationAxis = RotationAxis.X;
                    HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "X");
                    break;
                case 1: rotationAxis = RotationAxis.Y;
                    HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Y");
                    break;
                case 2: rotationAxis = RotationAxis.Z;
                    HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Z");
                    break;
            }
        }
    }
}
