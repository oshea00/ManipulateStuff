using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeManager : Singleton<GazeManager>
{
    [Tooltip("Maximum gaze distance for calculating a hit.")]
    public float MaxGazeDistance = 5.0f;

    [Tooltip("Select the layers raycast should target.")]
    public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

    /// <summary>
    /// Physics.Raycast result is true if it hits a Hologram.
    /// </summary>
    public bool Hit { get; private set; }

    /// <summary>
    /// HitInfo property gives access
    /// to RaycastHit public members.
    /// </summary>
    public RaycastHit HitInfo { get; private set; }

    /// <summary>
    /// Position of the user's gaze.
    /// </summary>
    public Vector3 Position { get; private set; }

    /// <summary>
    /// RaycastHit Normal direction.
    /// </summary>
    public Vector3 Normal { get; private set; }

    public GameObject FocusedGameObject { get; private set; }

    private GazeStabilizer gazeStabilizer;
    private Vector3 gazeOrigin;
    private Vector3 gazeDirection;

    void Awake()
    {
        gazeStabilizer = GetComponent<GazeStabilizer>();
    }

    private void Update()
    {
        gazeOrigin = Camera.main.transform.position;
        gazeDirection = Camera.main.transform.forward;
        gazeStabilizer.UpdateHeadStability(gazeOrigin, Camera.main.transform.rotation);
        gazeOrigin = gazeStabilizer.StableHeadPosition;
        UpdateRaycast();
        if (Hit)
        {
            if (HitInfo.collider != null)
            {
                FocusedGameObject = HitInfo.collider.gameObject;
                FocusedGameObject.SendMessageUpwards("GazeEntered");
                //Debug.Log("Gaze focused on " + FocusedGameObject.name);
            }
            else
            {
                if (FocusedGameObject != null)
                    FocusedGameObject.SendMessageUpwards("GazeExited");
                FocusedGameObject = null;
            }
        }
        else
        {
            if (FocusedGameObject != null)
                FocusedGameObject.SendMessageUpwards("GazeExited");
            FocusedGameObject = null;
        }
    }

    /// <summary>
    /// Calculates the Raycast hit position and normal.
    /// </summary>
    private void UpdateRaycast()
    {
        RaycastHit hitInfo;
        Hit = Physics.Raycast(gazeOrigin, gazeDirection, out hitInfo, MaxGazeDistance, RaycastLayerMask);
        HitInfo = hitInfo;
        if (Hit)
        {
            Position = HitInfo.point;
            Normal = HitInfo.normal;
        }
        else
        {
            Position = gazeOrigin + (MaxGazeDistance * gazeDirection);
            Normal = gazeDirection;
        }
    }
}