﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : UWEBase {
		//The cursor to display on the gui
		public RawImage MouseLookCursor;
		public Texture2D CursorIcon;
		public Texture2D CursorIconDefault;
		public Texture2D CursorIconBlank;
		public Texture2D CursorIconTarget; //Default Cursor for ranged spells and combat.



	/// Reference to the weapon animation animator.
	public WeaponAnimationPlayer wpa;

	public CutsceneAnimationFullscreen CutScenesFull;
	public ScrollController MessageScroll;
	public InputField InputControl;
	public GameObject main_window;//The immersive heads up display
	public RawImage MapDisplay; //should be in a subclass?

		public Text LoadingProgress;
}
