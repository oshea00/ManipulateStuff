using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class HandsManager : Singleton<HandsManager>
{
    /// <summary>
    /// Tracks the hand detected state.
    /// </summary>
    public bool HandDetected
    {
        get;
        private set;
    }

    // Keeps track of the GameObject that the hand is interacting with.
    public GameObject FocusedGameObject { get; private set; }

    void Awake()
    {
        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
        FocusedGameObject = null;
    }

    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        Debug.Log("Hand Detected");
        HandDetected = true;
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        HandDetected = false;
        ResetFocusedGameObject();
    }

    private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs hand)
    {
        Debug.Log("Hand Finger Pressed");
        if (GazeManager.Instance.FocusedGameObject != null)
        {
            Debug.Log("... On "+GazeManager.Instance.FocusedGameObject.name);
            FocusedGameObject = GazeManager.Instance.FocusedGameObject;
        }
        else
        {
            Debug.Log("... No Gaze focused");
        }
    }

    private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs hand)
    {
        ResetFocusedGameObject();
    }

    private void ResetFocusedGameObject()
    {
        FocusedGameObject = null;
    }

    void OnDestroy()
    {
        InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
        InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
    }
}