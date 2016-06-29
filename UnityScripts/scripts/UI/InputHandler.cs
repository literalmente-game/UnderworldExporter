﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputHandler : GuiBase {

		public const int  InputCharacterQty = 0;
		public const int  InputInventoryQty = 1;
		public const int  InputConversationWords = 2;
		public const int  InputMantraWords = 3;

		//For passing typed input to various objects

		public GameObject target;
		public int currentInputMode;

		public void OnSubmit()
		{
				int valueInt=0;
				string valueStr="";
				switch (currentInputMode)
				{
				case InputCharacterQty:
				case InputInventoryQty:
						valueInt=ParseInteger();
						break;
				case InputConversationWords:
				case InputMantraWords:
						valueStr=ParseString();
						break;
				}

				switch (currentInputMode)
				{
				case InputCharacterQty:
						target.gameObject.GetComponent<UWCharacter>().OnSubmitPickup(valueInt);
						break;
				case InputInventoryQty:
						target.gameObject.GetComponent<InventorySlot>().OnSubmitPickup(valueInt);
						break;		
				case InputConversationWords:
						target.gameObject.GetComponent<Conversation>().OnSubmitPickup(valueStr);
						break;
				case InputMantraWords:
						target.gameObject.GetComponent<Shrine>().OnSubmitPickup(valueStr);
						break;
				}

		}


		public int ParseInteger()
		{
				InputField inputctrl =playerUW.playerHud.InputControl;
				//Debug.Log (inputctrl.text);
				int quant=0;
				if (int.TryParse(inputctrl.text,out quant)==false)
				{
						return 0;
				}
				else
				{
						return quant;		
				}
		}

		public string ParseString()
		{
				return "";
		}

}
