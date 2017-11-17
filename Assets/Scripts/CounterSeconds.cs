using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterSeconds : MonoBehaviour {
    public static int count = 0;

    public int SecondsToDelay = 1;

	void Start () {
        StartCoroutine(EverySecond());
	}
	
	IEnumerator EverySecond()
    {
        while (true)
        {
            yield return new WaitForSeconds(SecondsToDelay);
            count++;
            NetworkClient.sendMessage = count.ToString();
        }

    }
}
