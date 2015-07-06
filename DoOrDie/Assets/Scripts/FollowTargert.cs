using UnityEngine;
using System.Collections;

public class FollowTargert : MonoBehaviour {

	Canvas myCanvas;
	// Use this for initialization
	void Start () {
		myCanvas=GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.y<0)
			myCanvas.enabled=false;
		else
			myCanvas.enabled=true;
			
		transform.eulerAngles=new Vector3(90,0,0);
	}
}
