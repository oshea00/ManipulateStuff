using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadAssetBundle : MonoBehaviour {
    GameObject _gameObject = null;
    public string bundleUri = "";
    public string objectName = "";
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

	void Start () {
        StartCoroutine(InstantiateObject());
	}
	
    IEnumerator InstantiateObject()

    {
        //string uri = "file:///" + "C:/unityprojects/Networking/Assets" + "/AssetBundles/" + "remote";
        //string uri = "http://10.0.0.206:8089/" + "remote";
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.GetAssetBundle(bundleUri, 0);
        yield return request.SendWebRequest();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        var gameObject = bundle.LoadAsset<GameObject>(objectName);
        _gameObject = Instantiate(gameObject);
        var childTransform = _gameObject.transform.Find("default");
        childTransform.gameObject.name = objectName;
        childTransform.gameObject.AddComponent<NavigationAction>().RotationSensitivity = 50f;
        var fingerPressed = Resources.Load("Sounds/FingerPressed", typeof(AudioClip)) as AudioClip;
        childTransform.gameObject.AddComponent<Interactible>().TargetFeedbackSound = fingerPressed;
#if UNITY_EDITOR
        childTransform.gameObject.AddComponent<MouseLook>();
#endif
        Material newMat = Resources.Load("Blue", typeof(Material)) as Material;
        var renderer = childTransform.gameObject.GetComponent<Renderer>();
        renderer.material = newMat;
        _gameObject.transform.position = position;
        _gameObject.transform.eulerAngles = rotation;
        _gameObject.transform.localScale = scale;
    }
}
