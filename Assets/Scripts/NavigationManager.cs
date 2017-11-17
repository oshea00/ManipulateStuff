using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class NavigationManager : Singleton<NavigationManager>
{
    // Tap and Navigation gesture recognizer.
    public GestureRecognizer NavigationRecognizer { get; private set; }
    public GestureRecognizer ManipulationRecognizer { get; private set;  }
    // Currently active gesture recognizer.
    public GestureRecognizer ActiveRecognizer { get; private set; }

    public bool IsNavigating { get; private set; }
    public Vector3 NavigationPosition { get; private set; }
    public bool IsManipulating { get; private set; }
    public Vector3 ManipulationPosition { get; private set; }

    void Awake()
    {
        NavigationRecognizer = new GestureRecognizer();
        NavigationRecognizer.SetRecognizableGestures(
            GestureSettings.NavigationX |
            GestureSettings.NavigationY |
            GestureSettings.NavigationZ |
            GestureSettings.ManipulationTranslate |
            GestureSettings.Tap
            );

        NavigationRecognizer.Tapped += NavigationRecognizer_Tapped;
        NavigationRecognizer.NavigationStarted += NavigationRecognizer_NavigationStarted;
        NavigationRecognizer.NavigationUpdated += NavigationRecognizer_NavigationUpdated;
        NavigationRecognizer.NavigationCompleted += NavigationRecognizer_NavigationCompleted;
        NavigationRecognizer.NavigationCanceled += NavigationRecognizer_NavigationCanceled;

        ManipulationRecognizer = new GestureRecognizer();
        ManipulationRecognizer.SetRecognizableGestures(
            GestureSettings.ManipulationTranslate|
            GestureSettings.Tap);
        ManipulationRecognizer.Tapped += NavigationRecognizer_Tapped;
        ManipulationRecognizer.ManipulationStarted += ManipulationRecognizer_ManipulationStarted;
        ManipulationRecognizer.ManipulationUpdated += ManipulationRecognizer_ManipulationUpdated;
        ManipulationRecognizer.ManipulationCompleted += ManipulationRecognizer_ManipulationCompleted;
        ManipulationRecognizer.ManipulationCanceled += ManipulationRecognizer_ManipulationCanceled;

        ResetGestureRecognizersToNavigation();
    }

    void OnDestroy()
    {
        // 2.b: Unregister the Tapped and Navigation events on the NavigationRecognizer.
        NavigationRecognizer.CancelGestures();
        NavigationRecognizer.StopCapturingGestures();
        NavigationRecognizer.Tapped -= NavigationRecognizer_Tapped;
        NavigationRecognizer.NavigationStarted -= NavigationRecognizer_NavigationStarted;
        NavigationRecognizer.NavigationUpdated -= NavigationRecognizer_NavigationUpdated;
        NavigationRecognizer.NavigationCompleted -= NavigationRecognizer_NavigationCompleted;
        // Unregister the Manipulation events on the ManipulationRecognizer.
        ManipulationRecognizer.CancelGestures();
        ManipulationRecognizer.StopCapturingGestures();
        ManipulationRecognizer.ManipulationStarted -= ManipulationRecognizer_ManipulationStarted;
        ManipulationRecognizer.ManipulationUpdated -= ManipulationRecognizer_ManipulationUpdated;
        ManipulationRecognizer.ManipulationCompleted -= ManipulationRecognizer_ManipulationCompleted;
        ManipulationRecognizer.ManipulationCanceled -= ManipulationRecognizer_ManipulationCanceled;
    }

    private void NavigationRecognizer_NavigationStarted(NavigationStartedEventArgs obj)
    {
        IsNavigating = true;
        IsManipulating = false;
        NavigationPosition = Vector3.zero;
        Debug.Log("Rotation Navigation Started");
    }

    private void NavigationRecognizer_NavigationUpdated(NavigationUpdatedEventArgs obj)
    {
        NavigationPosition = obj.normalizedOffset;
        Debug.Log("Rotation Navigation Updated");
    }

    private void NavigationRecognizer_NavigationCompleted(NavigationCompletedEventArgs obj)
    {
        IsNavigating = false;
        Debug.Log("Rotation Navigation Completed");
    }

    private void NavigationRecognizer_NavigationCanceled(NavigationCanceledEventArgs obj)
    {
        IsNavigating = false;
        Debug.Log("Rotation Navigation Canceled");
    }

    private void ManipulationRecognizer_ManipulationStarted(ManipulationStartedEventArgs obj)
    {
        Debug.Log("Manipulation Started");
        if (HandsManager.Instance.FocusedGameObject != null)
        {
            IsManipulating = true;
            IsNavigating = false;
            ManipulationPosition = Vector3.zero;
            HandsManager.Instance.FocusedGameObject.SendMessageUpwards("PerformManipulationStart", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationUpdated(ManipulationUpdatedEventArgs obj)
    {
        Debug.Log("Manipulation Updated");
        if (HandsManager.Instance.FocusedGameObject != null)
        {
            ManipulationPosition = obj.cumulativeDelta;
            HandsManager.Instance.FocusedGameObject.SendMessageUpwards("PerformManipulationUpdate", ManipulationPosition);
        }
    }

    private void ManipulationRecognizer_ManipulationCompleted(ManipulationCompletedEventArgs obj)
    {
        Debug.Log("Manipulation Completed");
        IsManipulating = false;
    }

    private void ManipulationRecognizer_ManipulationCanceled(ManipulationCanceledEventArgs obj)
    {
        Debug.Log("Manipulation Canceled");
        IsManipulating = false;
    }

    private void NavigationRecognizer_Tapped(TappedEventArgs obj)
    {
        Debug.Log("Navigation Tapped");
        GameObject focusedObject = GazeManager.Instance.FocusedGameObject;
        if (focusedObject != null)
        {
            focusedObject.SendMessage("OnSelect");
        }
    }

    public void ResetGestureRecognizersToNavigation()
    {
        Transition(NavigationRecognizer);
    }

    public void Transition(GestureRecognizer newRecognizer)
    {
        if (newRecognizer == null)
        {
            return;
        }

        if (ActiveRecognizer != null)
        {
            if (ActiveRecognizer == newRecognizer)
            {
                return;
            }

            ActiveRecognizer.CancelGestures();
            ActiveRecognizer.StopCapturingGestures();
        }

        newRecognizer.StartCapturingGestures();
        ActiveRecognizer = newRecognizer;
    }

}
