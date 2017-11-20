using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSendCoordinates : MonoBehaviour {

    void OnSelect() {
        NavigationManager.Instance.SendCoordinates = true;
        HUDText.Instance.axislabel = "Send Mode";
    }
}
