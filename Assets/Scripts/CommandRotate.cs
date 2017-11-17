using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandRotate : MonoBehaviour {

    private void OnSelect() {
        if (GazeManager.Instance.FocusedGameObject == gameObject)
        {
            NavigationManager.Instance.Transition(NavigationManager.Instance.NavigationRecognizer);
            Debug.Log("Rotate Selected");
        }
    }
}
