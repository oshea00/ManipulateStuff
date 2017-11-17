using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountWatch : MonoBehaviour {
	void Update () {
         Text msg = GetComponent<Text>();
         msg.text = CounterSeconds.count.ToString();
    }
}
