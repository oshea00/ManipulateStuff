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
    /// Position of the user's gaze in World.
    /// </summary>
    public Vector3 Position { get; private set; }

    /// <summary>
    /// Postion of the user's gave on Focused object in local frame.
    /// </summary>
    public Vector3 LocalPosition { get; private set; }

    /// <summary>
    /// RaycastHit Normal direction.
    /// </summary>
    public Vector3 Normal { get; private set; }

    public GameObject FocusedGameObject { get; private set; }

    private GazeStabilizer gazeStabilizer;
    public Vector3 GazeOrigin;
    public Vector3 GazeDirection;
    public Vector3 GazeAngles;

    void Awake()
    {
        gazeStabilizer = GetComponent<GazeStabilizer>();
    }

    private void Update()
    {
        GazeOrigin = Camera.main.transform.position;
        GazeDirection = Camera.main.transform.forward;
        GazeAngles = Camera.main.transform.eulerAngles;
        gazeStabilizer.UpdateHeadStability(GazeOrigin, Camera.main.transform.rotation);
        GazeOrigin = gazeStabilizer.StableHeadPosition;
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
        Hit = Physics.Raycast(GazeOrigin, GazeDirection, out hitInfo, MaxGazeDistance, RaycastLayerMask);
        HitInfo = hitInfo;
        if (Hit)
        {
            Position = HitInfo.point;
            LocalPosition = HitInfo.collider.gameObject.transform.InverseTransformPoint(Position);
            Normal = HitInfo.normal;
        }
        else
        {
            Position = GazeOrigin + (MaxGazeDistance * GazeDirection);
            LocalPosition = Vector3.zero;
            Normal = GazeDirection;
        }
    }
}