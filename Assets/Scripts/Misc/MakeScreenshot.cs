using UnityEngine;
using System.Collections;

public class MakeScreenshot : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}


	public void Capture()
	{
		Application.CaptureScreenshot("Screenshot.png", 2);
		
	}

}
