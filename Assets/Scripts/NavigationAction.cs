using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Popup info graphic triggered by proximity to a given object point
// TODO: Populate feature points on unity 3D ltas2 asset
// TODO: Use surface mesh to raycast tracker location to SA
// TODO: Display tracker in Hololens world
// TODO: Menu command to level current active rotation axis
// TODO: Retrieve adjusted dx,dy,dz,drx,dry,drz values from SA and update unity model transform

public class NavigationAction : MonoBehaviour {
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 100.0f;
    public float MoveSensitivity = 2f;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis rotationAxis = RotationAxis.X;

    private int nextAxis = 0;
    private Vector3 manipulationPreviousPosition;

    private void Start()
    {
        if (GazeManager.Instance.FocusedGameObject != null)
        {
            HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "X");
        }
        else
        {
            HUDText.Instance.axislabel = string.Format("{0}", "NONE");
        }
    }

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
                HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "X");
                transform.Rotate(rotationFactor, 0, 0);
                break;
            case RotationAxis.Y:
                HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Y");
                transform.Rotate(0, rotationFactor, 0);
                break;
            case RotationAxis.Z:
                HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Z");
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
        if (NavigationManager.Instance.SendCoordinates == false)
        {
            if (GazeManager.Instance.FocusedGameObject == gameObject)
            {
                nextAxis++;
                nextAxis = nextAxis % 3;
                Debug.Log("Next axis = " + nextAxis);
                switch (nextAxis)
                {
                    case 0:
                        rotationAxis = RotationAxis.X;
                        HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "X");
                        break;
                    case 1:
                        rotationAxis = RotationAxis.Y;
                        HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Y");
                        break;
                    case 2:
                        rotationAxis = RotationAxis.Z;
                        HUDText.Instance.axislabel = string.Format("{0} {1}", gameObject.name, "Z");
                        break;
                }
            }
        }
        else
        {
            var p = GazeManager.Instance.Position;
            string msg = string.Format("WORLD:{0:0.0},{1:0.0},{2:0.0}", p.z * 39.37, -p.x * 39.37, p.y * 39.37);
            NetworkClient.Instance.Send(msg);
        }
    }
}
