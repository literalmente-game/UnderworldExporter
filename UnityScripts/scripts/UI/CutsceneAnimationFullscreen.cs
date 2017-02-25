﻿using UnityEngine;
using System.Collections;

public class CutsceneAnimationFullscreen : HudAnimation {
	private bool PlayingSequence;
	private bool isFullScreen;
	private int currentFrameLoops;
	public Cuts cs;
	public string CurrentSpriteName;
	Sprite CurrentSpriteLoaded;
	public ScrollController mlCuts; //What control will print the subtitles
	public AudioSource aud; //What control will play the audio.
	public bool SkipAnim=false;
	private float CutsceneTime;
	public Sprite filler;

		public string currentCutsFile;
		public string previousCutsFile;

		public void End()
		{
			if (cs!=null)
			{
				cs.PostCutSceneEvent();
				PlayingSequence=false;
				UWHUD.instance.EnableDisableControl(UWHUD.instance.CutsceneFullPanel.gameObject,false);
				Destroy (cs);	
			}
		}

	public void Begin()
	{
		if (cs==null)
		{
			return;
		}
		CutsceneTime=0f;

		UWHUD.instance.EnableDisableControl(UWHUD.instance.CutsceneFullPanel,true);
		PlayingSequence=true;
		SkipAnim=false;
		cs.PreCutsceneEvent();
		//Begin the image seq
		StartCoroutine(PlayCutsImageSequence());
		//Begin the subs seq
		StartCoroutine(PlayCutsSubtitle());
		//Begin the audio seq
		StartCoroutine(PlayCutsAudio());
	}
	
	IEnumerator PlayCutsImageSequence()
	{
		float currTime=0.0f;
		for (int i = 0; i<cs.NoOfImages(); i++)
		{
			yield return new WaitForSeconds(cs.getImageTime(i)-currTime);   //Wait until it is time for the frame
			currTime = cs.getImageTime(i);                                   //For the next wait.
			currentFrameLoops = cs.getImageLoops(i);
			if (currentFrameLoops!=0)
			{
				anim.looping=true;
			}
			else
			{
				anim.looping=false;
			}
			SetAnimation= cs.getImageFrame(i);	
		}
		SetAnimation= "Anim_Base";//End of anim.
		PlayingSequence=false;
		PostAnimPlay();
		End();
	}

	public void Loop()
	{ //Handles the looping of frames for the images.
		switch (currentFrameLoops)
		{
		case -1://Loop forever until interupted. Assumes anim is already looping.
			break;
		case 0://Loop has completed. Switch to black anim
			SetAnimation= cs.getFillerAnim();
			break;
		default:
			currentFrameLoops--;  //Assumes animation is already looping.
			break;
		}
	}
	
	IEnumerator PlayCutsSubtitle()
	{
		mlCuts.Set("");
		float currTime=0.0f;
		for (int i = 0; i<=cs.getNoOfSubs(); i++)
		{
			yield return new WaitForSeconds(cs.getSubTime(i)-currTime);
			currTime = cs.getSubTime(i)+cs.getSubDuration (i);//The time the subtitle finishes at.
            if (i<cs.getNoOfSubs())
            {//A new sub is due before this one will finish.
                currTime= Mathf.Min(currTime,cs.getSubTime(i+1));
            }
			mlCuts.Set(StringController.instance.GetString(cs.StringBlockNo, cs.getSubIndex(i)));
			yield return new WaitForSeconds(cs.getSubDuration (i));
			mlCuts.Set("");//Clear the control.
		}
	}
	
	IEnumerator PlayCutsAudio()
	{
		float currTime=0.0f;
		for (int i = 0; i<cs.getNoOfAudioClips(); i++)
		{
			yield return new WaitForSeconds(cs.getAudioTime (i)-currTime);
			currTime =cs.getAudioTime (i);
			aud.clip = Resources.Load <AudioClip>(cs.getAudioClip(i));
			aud.loop=false;
			aud.Play();
		}
	}
  



	/*Functions for handling the end of cutscene clip events*/
	public void PreAnimPlay()
	{//Called by events in certain animations when starting playing
		//if (PlayingSequence==false)
		//{
			//GameWorldController.instance.playerUW.playerCam.cullingMask=0;//Stops the camera from rendering.
			//chains.ActiveControl=5;
			//UWHUD.instance.RefreshPanels(PANELNAME);

			//isFullScreen= UWHUD.instance.window.FullScreen;
			//if (!isFullScreen)
			//{
			//	UWHUD.instance.window.SetFullScreen();
			//}//
		//}		
		return;
	} 
	
	public void PostAnimPlay()
	{
		if ((PlayingSequence==false) || (cs==null))
		{
			//if (!isFullScreen)
			//{
			//	UWHUD.instance.window.UnSetFullScreen();
			//}
			//GameWorldController.instance.playerUW.playerCam.cullingMask=HudAnimation.NormalCullingMask;//?
				

			//chains.ActiveControl=0;
			//UWHUD.instance.RefreshPanels(PANELNAME);
			SetAnimation= "Anim_Base";//Clears out the animation.
			mlCuts.Set("");
			UWHUD.instance.EnableDisableControl(UWHUD.instance.CutsceneFullPanel,false);			
		}
		else
		{
			Loop ();
		}
	}

	protected override void Update()
	{
		//if (SetAnimation!=PreviousAnimation)
		//{//Load the next cutscene file
		//	InitCutsFile();
		//}
		base.Update();
		if (PlayingSequence)
		{
			CutsceneTime+=Time.deltaTime;
			if (Input.anyKey)
			{
					if (CutsceneTime>=3.0f)
					{//Only end a cutscene if it has been running for longer than 3 seconds
							SetAnimation= "Anim_Base";//End of anim.
							PlayingSequence=false;
							PostAnimPlay();
							StopAllCoroutines();	
							//TargetControl.gameObject.SetActive(false);
							UWHUD.instance.EnableDisableControl(UWHUD.instance.CutsceneFullPanel.gameObject,false);
							Destroy (cs);	
					}
			}
		}
	}


		void InitCutsFile()
		{
			if(SetAnimation!="Anim_Base")
			{
				GameWorldController.instance.cutsLoader= new CutsLoader(SetAnimation.Replace("_","."));
			}
		}
	
		/*
		/// <summary>
	/// Deactivates the fullscreen animation if clicked on during anim_base
	/// </summary>
		public void LateUpdate()
		{
			if (sprt==null)
			{
				sprt=this.GetComponentInChildren<SpriteRenderer>();	
			}

			if(CurrentSpriteName != sprt.sprite.name)
			{
				CurrentSpriteName= sprt.sprite.name;
				switch (sprt.sprite.name)
					{
					case "cuts_blank":
								if (Cuts.filler==null)
								{
										CurrentSpriteLoaded= filler;	
								}
								else
								{
								CurrentSpriteLoaded= Cuts.filler.RequestSprite(0);	
								}
						break;
					case "SplashOrigin":
					case "pres1_0000":
						Texture2D img= GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRES1_BYT);
						CurrentSpriteLoaded=Sprite.Create(img,new Rect(0,0,img.width,img.height), new Vector2(0.5f, 0.0f));							
						break;
					case "SplashBlueSky":
					case "pres2_0000":
						Texture2D img1= GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRES2_BYT);
						CurrentSpriteLoaded=Sprite.Create(img1,new Rect(0,0,img1.width,img1.height), new Vector2(0.5f, 0.0f));
						break;
					case "SplashOriginDemo":
							Texture2D img2= GameWorldController.instance.bytloader.LoadImageAt(BytLoader.PRESD_BYT);
							CurrentSpriteLoaded=Sprite.Create(img2,new Rect(0,0,img2.width,img2.height), new Vector2(0.5f, 0.0f));							
							break;	
					default:
						currentCutsFile = CurrentSpriteName.Substring(0,9);
						if (currentCutsFile!=previousCutsFile)
						{
							GameWorldController.instance.cutsLoader= new CutsLoader(currentCutsFile.Replace("_","."));
						}
						previousCutsFile=currentCutsFile;
						int SpriteNo;
						if (int.TryParse(CurrentSpriteName.Substring(CurrentSpriteName.Length-4,4),out SpriteNo))
						{
							CurrentSpriteLoaded= GameWorldController.instance.cutsLoader.RequestSprite(SpriteNo);	
						}
						else
						{
							Debug.Log("Unable to find sprite index for " + CurrentSpriteName + " in animation " + SetAnimation);
						}	
						break;
					}
			}

				if (sprt!=null)
				{
						sprt.sprite=CurrentSpriteLoaded;
						if (sprt.sprite!=null)
						{
								TargetControl.texture =sprt.sprite.texture;		
						}	
				}

		}*/

}