using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWatcher : MonoBehaviour {
	void Update () {
        Text msg = GetComponent<Text>();
        msg.text = NetworkClient.rcvdMessage;
	}
}
