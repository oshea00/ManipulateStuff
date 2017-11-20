using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMove : MonoBehaviour {

    private void OnSelect()
    {
        if (GazeManager.Instance.FocusedGameObject == gameObject)
        {
            NavigationManager.Instance.Transition(NavigationManager.Instance.ManipulationRecognizer);
            NavigationManager.Instance.SendCoordinates = false;
            Debug.Log("Move Selected");
        }
    }	
	
}
