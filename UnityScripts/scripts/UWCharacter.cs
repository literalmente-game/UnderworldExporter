﻿using UnityEngine;
using System.Collections;

/*
The basic character. Stats and interaction.
 */ 
public class UWCharacter : Character {

	//What magic spells are currently active on (and cast by) the player. (max 3)
	//These are the ones that the player can see on the hud.
	public SpellEffect[] ActiveSpell=new SpellEffect[3]; 

	//What effects and enchantments (eg from items are equipped on the player)
	public SpellEffect[] PassiveSpell=new SpellEffect[10];

	public int Body;//Which body/portrait this character has 

	public UWHUD playerHud;

	//Character related info
	//Character Details

	public string CharClass;
	public int CharLevel;
	public int EXP;
	public bool isFemale;
	public bool isLefty;
	public bool isSwimming;
	public bool isFlying;
	public bool isFloating;
	public bool isWaterWalking;
	public bool onGround;//Not currently used.
	public bool isTelekinetic;

	//Character Status
	public int FoodLevel;
	public int Fatigue;
	public bool Poisoned;

	//Character skills
	public Skills PlayerSkills;

	//Magic system
	public Magic PlayerMagic;

	//Inventory System
	public PlayerInventory playerInventory;
	
	//Combat System
	public UWCombat PlayerCombat;

	public long summonCount=0;//How many stacks I have split so far. To keep them uniquely named.

	public int ResurrectLevel;
	public Vector3 ResurrectPosition=Vector3.zero;

	public Vector3 MoonGatePosition=Vector3.zero;
	public int MoonGateLevel = 2;//Domain of the mountainmen

	public override void Start ()
	{
		base.Start ();

		XAxis.enabled=false;
		YAxis.enabled=false;
		MouseLookEnabled=false;
		Cursor.SetCursor (CursorIconBlank,Vector2.zero, CursorMode.ForceSoftware);
		GuiBase.playerUW = this.gameObject.GetComponent<UWCharacter>();
		InteractionMode=UWCharacter.DefaultInteractionMode;


		//Tells other objects about this component;
		InventorySlot.playerUW=this.GetComponent<UWCharacter>();

		DoorControl.playerUW=this.gameObject.GetComponent<UWCharacter>();
		Container.playerUW=this.GetComponent<UWCharacter>();
		ContainerOpened.playerUW =this.GetComponent<UWCharacter>();
		ActiveRuneSlot.playerUW=this.GetComponent<UWCharacter>();
		SpellEffect.playerUW=this.GetComponent<UWCharacter>();
		SpellEffectsDisplay.playerUW=this.GetComponent<UWCharacter>();
		RuneSlot.playerUW=this.GetComponent<UWCharacter>();
		Eyes.playerUW=this.GetComponent<UWCharacter>();
		NPC.playerUW=this.GetComponent<UWCharacter>();

		object_base.playerUW= this.gameObject.GetComponent<UWCharacter>();
		Conversation.playerUW = this.gameObject.GetComponent<UWCharacter>();

		StringControl.InitStringController(Application.dataPath + "//..//uw1_strings.txt");

		ObjectInteraction.playerUW =this.gameObject.GetComponent<UWCharacter>();
		playerHud.InputControl.text="";
		playerHud.InputControl.label.text="";
		playerHud.MessageScroll.Clear ();
	}

	void PlayerDeath()
	{//CHeck if the player has planted the seed and if so send them to that position.
		mus.Death=true;
		if ( playerHud.CutScenesSmall!=null)
		{
			if (ResurrectPosition!=Vector3.zero)
			{
				 playerHud.CutScenesSmall.SetAnimation="Death_With_Sapling";
			}
			else
			{
				playerHud.CutScenesSmall.SetAnimation="Death";
			}
		}

		if (ResurrectPosition!=Vector3.zero)
		{
			this.transform.position=ResurrectPosition;
		}
		//Cancel the spell
		if (PlayerMagic.ReadiedSpell!="")
		{
			PlayerMagic.ReadiedSpell="";
			CursorIcon=CursorIconDefault;
		}
	}

	public override float GetUseRange ()
	{
		if (isTelekinetic==true)
		{
			return useRange*8.0f;
		}
		else
		{

			if (playerInventory.GetObjectInHand() =="")
			{
				return useRange;
			}
			else
			{//Test if this is a pole. If so extend the use range by a small amount.
				ObjectInteraction objIntInHand = playerInventory.GetGameObjectInHand().GetComponent<ObjectInteraction>();
				if (objIntInHand!=null)
				{
					switch (objIntInHand.ItemType)
					{
						case ObjectInteraction.POLE:
							return useRange *2;
							break;
					}
				}
				return useRange;
			}
		}
	}


	public override float GetPickupRange ()
	{
		if (isTelekinetic==true)
		{
			return pickupRange*8.0f;
		}
		else
		{
			return pickupRange;
		}
	}


	// Update is called once per frame
	public override void Update () {
		base.Update ();
		if (WindowDetectUW.WaitingForInput==true)
		{//TODO: This should be in window detect
			//playerHud.MessageScroll.gameObject.GetComponent<UIInput>().selected=true;
			//playerHud.MessageScroll.gameObject.GetComponent<UIInput>().selected=true;
			playerHud.InputControl.selected=true;
		}
		if ((CurVIT<=0) && (mus.Death==false))
		{

			PlayerDeath();
			return;
		}
		if(mus.Death==true)
		{
			//Still processing death.
			return;
		}

		if (playerCam.enabled==true)
		{
			if (isSwimming==true)
			{
				Camera.main.transform.localPosition=new Vector3(Camera.main.transform.localPosition.x,-1.0f,Camera.main.transform.localPosition.z);
			}
			else
			{
				Camera.main.transform.localPosition=new Vector3(Camera.main.transform.localPosition.x,0.9198418f,Camera.main.transform.localPosition.z);
			}
		}

		if (isFlying)
		{
			playerMotor.movement.maxFallSpeed=0.0f;
		}
		else
		{
			if (isFloating)
			{
				playerMotor.movement.maxFallSpeed=0.1f;//Default
			}
			else
			{
				playerMotor.movement.maxFallSpeed=20.0f;//Default
			}
		}

		mus.WeaponDrawn=(InteractionMode==UWCharacter.InteractionModeAttack);

		if (PlayerMagic.ReadiedSpell!="")
		{//Player has a spell thats about to be cast. All other activity is ignored.
			SpellMode ();
			return;
		}

		PlayerCombat.PlayerCombatUpdate();


	
	}

	public void SpellMode()
	{//Casts a spell on right click.
		if(Input.GetMouseButtonDown(1) && (WindowDetectUW.CursorInMainWindow==true))
		{
			PlayerMagic.castSpell(this.gameObject, PlayerMagic.ReadiedSpell,false);
			PlayerMagic.SpellCost=0;
		}
	}


	public void OnSubmitPickup()
	{

		//Debug.Log ("Value summited");

		UIInput inputctrl =playerHud.InputControl;//playerHud.MessageScroll.gameObject.GetComponent<UIInput>();
		//Debug.Log (inputctrl.text);
		int quant=0;
		if (int.TryParse(inputctrl.text,out quant)==false)
		{
			quant=0;
		}
		Time.timeScale=1.0f;
		WindowDetectUW.WaitingForInput=false;
		inputctrl.text="";
		inputctrl.label.text="";
		playerHud.MessageScroll.Clear ();
		//int quant= int.Parse(inputctrl.text);
		if (quant==0)
		{//cancel
			QuantityObj=null;
		}
		if (QuantityObj!=null)
			{
			if (quant >= QuantityObj.Link)
				{
				Pickup(QuantityObj,playerInventory);
				}
			else
				{
				//split the obj.
				GameObject Split = Instantiate(QuantityObj.gameObject);//What we are picking up.
				Split.GetComponent<ObjectInteraction>().Link =quant;
				Split.name = Split.name+"_"+summonCount++;
				QuantityObj.Link=QuantityObj.Link-quant;
				Pickup (Split.GetComponent<ObjectInteraction>(), playerInventory);
				QuantityObj=null;//Clear out to avoid weirdness.
				}
			}
	}

	public void TalkMode()
	{//Talk to the object clicked on.
		Ray ray ;
		if (MouseLookEnabled==true)
		{
			ray =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		}
		else
		{
			//ray= Camera.main.ViewportPointToRay(Input.mousePosition);
			ray= Camera.main.ScreenPointToRay(Input.mousePosition);
		}

		RaycastHit hit = new RaycastHit(); 
		if (Physics.Raycast(ray,out hit,talkRange))
		{
			if (hit.transform.gameObject.GetComponent<ObjectInteraction>()!=null)
				{
				hit.transform.gameObject.GetComponent<ObjectInteraction>().TalkTo();
				}
		}
		else
		{
			playerHud.MessageScroll.Add ("Talking to yourself?");
		}
	}

	public override void LookMode ()
		{//Look at the clicked item.
			Ray ray ;
			if (MouseLookEnabled==true)
			{
				ray =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			}
			else
			{
				ray= Camera.main.ScreenPointToRay(Input.mousePosition);
			}
			
			RaycastHit hit = new RaycastHit(); 
			if (Physics.Raycast(ray,out hit,lookRange))
			{
				//Debug.Log ("Hit made" + hit.transform.name);
				ObjectInteraction objInt = hit.transform.GetComponent<ObjectInteraction>();
				if (objInt != null)
				{
					objInt.LookDescription();
					return;
				}
				else
				{
				int len = hit.transform.name.Length;
				if (len >4){len=4;}
					switch(hit.transform.name.Substring(0,len).ToUpper())
					{
					case "CEIL":
						playerHud.MessageScroll.Add ("You see the ceiling");
						//GetMessageLog().text = "You see the ceiling";
						break;	
					case "PILL":
						//GetMessageLog().text = 
						playerHud.MessageScroll.Add("You see a pillar");
						break;	
					case "BRID":
						//000~001~171~You see a bridge.
						//GetMessageLog().text= 
						playerHud.MessageScroll.Add(StringControl.GetString(1,171));
						break;
					case "WALL":
					case "TILE":
					default:
						if (hit.transform.GetComponent<PortcullisInteraction>()!=null)
							{
								ObjectInteraction objPicked = hit.transform.GetComponent<PortcullisInteraction>().getParentObjectInteraction();
								if (objPicked!=null)
								{
									objPicked.LookDescription ();
								}
								return;
							}
					//Taken from
					//http://forum.unity3d.com/threads/get-material-from-raycast.53123/
					Renderer rend = hit.collider.GetComponent<Renderer>();
					if (rend ==null)
					{
						return;
					}
					MeshCollider meshCollider = (MeshCollider)hit.collider;
					int materialIndex = -1;
					Mesh mesh = meshCollider.sharedMesh;
					int triangleIdx = hit.triangleIndex;
					int lookupIdx1 = mesh.triangles[triangleIdx*3];
					int lookupIdx2 = mesh.triangles[triangleIdx*3+1];
					int lookupIdx3 = mesh.triangles[triangleIdx*3+2];
					int submeshNr = mesh.subMeshCount;

					for (int i = 0; i< submeshNr; i++)
					{
						int[] tr = mesh.GetTriangles(i);
						for (int j = 0; j<tr.Length; j+=3)
						{
							if ((tr[j] == lookupIdx1) && (tr[j+1]== lookupIdx2) && (tr[j+2])== lookupIdx3)
							{
								materialIndex=i;
								break;
							}
						}
						if (materialIndex!=-1)
						{
							break;
						}
					}
					if (materialIndex!=-1)
					{
						if (rend.materials[materialIndex].name.Length>=7)
						{
							int textureIndex =0;
							if (int.TryParse(rend.materials[materialIndex].name.Substring(4,3),out textureIndex))//int.Parse(rend.materials[materialIndex].name.Substring(4,3));
							{
								//GetMessageLog ().text =
								playerHud.MessageScroll.Add("You see " + StringControl.GetTextureName(textureIndex));
							}
						}
					//	GetMessageLog().text=rend.materials[materialIndex].name;

						//Debug.Log (rend.materials[materialIndex].name.Substring(4,3));
					}
					break;

					}
				}
			}
		}


	public override void PickupMode ()
	{
		//Picks up the clicked object in the view.
		PlayerInventory pInv = this.GetComponent<PlayerInventory>();
		if (InvMarker==null)
		{
			InvMarker=GameObject.Find ("InventoryMarker");
		}
		if (pInv.ObjectInHand=="")//Player is not holding anything.
		{//Find the object within the pickup range.
			Ray ray ;
			if (MouseLookEnabled==true)
			{
				ray =Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
			}
			else
			{
				ray= Camera.main.ScreenPointToRay(Input.mousePosition);
			}
			RaycastHit hit = new RaycastHit(); 
			if (Physics.Raycast(ray,out hit,GetPickupRange()))
			{
				ObjectInteraction objPicked;
				objPicked=hit.transform.GetComponent<ObjectInteraction>();
				if (objPicked!=null)//Only objects with ObjectInteraction can be picked.
				{
					if (objPicked.CanBePickedUp==true)
					{
						if (UICamera.currentTouchID==-2)
						{
							//right click check for quant.
							//Pickup if either not a quantity or is a quantity of one.
							if ((objPicked.isQuant ==false) || ((objPicked.isQuant)&&(objPicked.Link==1)) || (objPicked.isEnchanted))
							{
								Pickup(objPicked,pInv);
							}
							else
							{
								//Debug.Log("attempting to pick up a quantity");

								playerHud.MessageScroll.Set ("Move how many?");
								UIInput inputctrl =playerHud.InputControl;//playerHud.MessageScroll.GetComponent<UIInput>();
								inputctrl.GetComponent<GuiBase>().SetAnchorX(0.3f);
								inputctrl.eventReceiver=this.gameObject;
								inputctrl.type=UIInput.KeyboardType.NumberPad;
								inputctrl.text="1";
								inputctrl.label.text="1";
								inputctrl.selected=true;
								QuantityObj=objPicked;	
								Time.timeScale=0.0f;
								WindowDetect.WaitingForInput=true;
							}		
						}
						else
						{//Left click. Pick them all up.
							Pickup(objPicked,pInv);	
						}						
					}
				}
			}
		}
	}

	public Quest quest()
	{
		return this.GetComponent<Quest>();
	}
}
