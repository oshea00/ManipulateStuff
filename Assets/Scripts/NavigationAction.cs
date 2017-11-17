using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Cursor overlap bug
// TODO: NavigationAction move sensitivity
// TODO: Rotation - clamp depending on cumulative delta
// TODO: Model should be leveled - limit to rotation around Y....
// TODO: Button tap feedback
// TODO: Show current rotation axis on HUD for gazed object
// TODO: Menu tagalong...

public class NavigationAction : MonoBehaviour {
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 10.0f;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.X;

    private int nextAxis = 0;
    private Vector3 manipulationPreviousPosition;

    private float rotationFactor;

    void Update()
    {
        if (NavigationManager.Instance.IsNavigating && HandsManager.Instance.FocusedGameObject == gameObject)
        {
            PerformRotation();
        }
    }

    private void PerformRotation()
    {
        rotationFactor = NavigationManager.Instance.NavigationPosition.x * RotationSensitivity;
            
        switch (rotationAxis)
        {
            case RotationAxis.X:
                transform.Rotate(new Vector3(-1 * rotationFactor, 0, 0));
                break;
            case RotationAxis.Y:
                transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
                break;
            case RotationAxis.Z:
                transform.Rotate(new Vector3(0, 0, -1 * rotationFactor));
                break;
        }
    }

    void PerformManipulationStart(Vector3 position)
    {
        if (HandsManager.Instance.FocusedGameObject == gameObject)
        {
            Debug.Log("Manipulation Start");
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
                transform.position += moveVector;
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
                case 0: rotationAxis = RotationAxis.X; break;
                case 1: rotationAxis = RotationAxis.Y; break;
                case 2: rotationAxis = RotationAxis.Z; break;
            }
        }
    }
}
