﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class CutsAnimator : GuiBase {
		public bool Reset;
		public int frameIndex;
		public Texture2D anim_base;//Transparent image
		public Texture2D black; 
		public RawImage TargetControl;
		public string SetAnimation="";
		public string PrevAnimation="";
		bool mode;//True if using cs file
		public bool looping;
		BytLoader bty;
		CutsLoader cuts;

	public override void Start ()
	{
		base.Start ();
		TargetControl.texture=anim_base;
		mode=false;
	}


	void Update()
		{
			if (PrevAnimation!=SetAnimation)	
			{
				PlayAnimFile(SetAnimation);
				PrevAnimation=SetAnimation;
			}
		}

	IEnumerator cutscenerunner()
	{
		while (Reset==false)
		{
			if (mode==true)//Playing a cuts file
			{					
				if (frameIndex>cuts.ImageCache.GetUpperBound(0))
				{	
					if (looping)
					{
						frameIndex=0;
						TargetControl.texture= cuts.ImageCache[frameIndex++];
					}
					else
					{//fade to black
						TargetControl.texture=black;
						PostAnimPlay();
					}						
				}
				else
				{
					TargetControl.texture= cuts.ImageCache[frameIndex++];	
				}
				
			}
			else
				{//Showing a static image
					//Do nothing/quit out of routine.
					Reset=true;
				}

			yield return new WaitForSeconds(0.2f);
		}
	}


		void PostAnimPlay()
		{//Code to call at the end of some animations.
				switch (SetAnimation.ToLower())	
				{
				case "death_with_sapling":
						{
							if (GameWorldController.instance.getMus()!=null)
							{
									GameWorldController.instance.playerUW.CurVIT=GameWorldController.instance.playerUW.MaxVIT;
									GameWorldController.instance.getMus().Death=false;
									GameWorldController.instance.getMus().Combat=false;
									GameWorldController.instance.getMus().Fleeing=false;
									MusicController.LastAttackCounter=0.0f;
							}
							//maincam.enabled=true;
							GameWorldController.instance.playerUW.playerCam.cullingMask=HudAnimation.NormalCullingMask;
							SetAnimation= "Anim_Base";//Clears out the animation.

							if (GameWorldController.instance.LevelNo!=GameWorldController.instance.playerUW.ResurrectLevel-1)
							{
									GameWorldController.instance.SwitchLevel(GameWorldController.instance.playerUW.ResurrectLevel-1);
							}
							GameWorldController.instance.playerUW.gameObject.transform.position=GameWorldController.instance.playerUW.ResurrectPosition;
							break;
						}

				case "death":
					//PlayAnimFile("death_final");
					SetAnimation="death_final";
					break;
				case "death_final":
						Debug.Log("return to main menu..");
						break;
				}
		}

		void PlayAnimFile(string animName)
		{
			Reset=true;
			frameIndex=0;
			StopAllCoroutines();
			switch (SetAnimation.ToLower())
			{	
			
			case "fadetoblacksleep":
				TargetControl.texture=black;
				break;	
			case "cuts_blank":
			case "":
			case "anim_base":
				TargetControl.texture=anim_base;
				break;	
			case "splashlookingglass":
				TargetControl.texture = (Texture2D)GameWorldController.instance.bytloader.LoadImageAt(7);			
				break;
			case "splashoriginea":
				TargetControl.texture = (Texture2D)GameWorldController.instance.bytloader.LoadImageAt(6);			
				break;
			case "splashorigin":
			case "pres1_0000":
				TargetControl.texture = (Texture2D)GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRES1_BYT);			
				break;
			case "splashbluesky":
			case "pres2_0000":
				TargetControl.texture = (Texture2D)GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRES2_BYT);
				break;
			case "splashorigindemo":
				TargetControl.texture = (Texture2D)GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRESD_BYT);		
				break;	
			case "almricsitting"://special case
				cuts = new CutsLoader("cs000.n11");
				TargetControl.texture = cuts.ImageCache[0];
				break;
			case "death_with_sapling":
					cuts = new CutsLoader("cs402.n01");
						mode=true;Reset=false;
					TargetControl.texture = cuts.ImageCache[0];
						StartCoroutine (cutscenerunner());		
					break;
			case "death":
					cuts = new CutsLoader("cs403.n01");
						mode=true;Reset=false;
					TargetControl.texture = cuts.ImageCache[0];
						StartCoroutine (cutscenerunner());		
					break;
			case "death_final":
					cuts = new CutsLoader("cs403.n02");
					TargetControl.texture = cuts.ImageCache[0];
						mode=true;Reset=false;
						StartCoroutine (cutscenerunner());		
					break;
			case "ChasmMap":
				cuts = new CutsLoader("cs410.n01");
				TargetControl.texture = cuts.ImageCache[0];
				break;	
			case "Anvil":
				cuts = new CutsLoader("cs404.n01");
				TargetControl.texture = cuts.ImageCache[0];
						StartCoroutine (cutscenerunner());		
				break;
			//case "c401.n01":

			default:
				if (SetAnimation.Substring(0,7) == "Volcano")
				{
					cuts = new CutsLoader("cs400.n01");
					int index= int.Parse(SetAnimation.Substring(SetAnimation.Length-1,1));
					TargetControl.texture = cuts.ImageCache[index];
				}
				else if ((SetAnimation.Substring(0,5) == "Grave"))
				{//Graves
						cuts = new CutsLoader("cs401.n01");
						//TargetControl.texture = cuts.ImageCache[0];
						//break;	
					string regexForNumber="([-+]?[0-9]*\\.?[0-9]+)";
					Match GraveID =  Regex.Match(SetAnimation, regexForNumber);
					if (GraveID.Success)
					{
						int value = int.Parse(GraveID.Groups[0].Value);
						TargetControl.texture = cuts.ImageCache[value];	
					}
				}
				else
				{
					mode=true;
					Reset=false;
					cuts = new CutsLoader(animName.Replace("_","."));
					StartCoroutine (cutscenerunner());		
				}

				break;
			}
		}
}
