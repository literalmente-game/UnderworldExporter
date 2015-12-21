﻿using UnityEngine;
using System.Collections;

public class MapClose : MonoBehaviour {

	void OnClick()
	{
		GameObject map = GameObject.Find ("MapAnchor");
		
		//Turn on the camera
		foreach(Transform child in map.transform)
		{
			if (child.name == "MapPanel")
			{
				child.gameObject.SetActive(false);
			}
		}
		WindowDetect.InMap=false;
		//Turn off the main hud
		//GameObject UWHud =GameObject.Find ("UW_HUD");
		//foreach(Transform child in UWHud.transform)
		//{
		//	if ((child.name == "Anchor")||(child.name == "Camera"))
		//	{
		//		child.gameObject.SetActive(true);
		//	}
		//}
		GameObject mus = GameObject.Find ("MusicController");
		if  (mus!=null)
		{
			mus.GetComponent<MusicController>().InMap=false;
		}
		chains.ActiveControl=0;
		chains.Refresh();
	}
}
