﻿using UnityEngine;
using System.Collections;

public class Spike : object_base {

public override bool use ()
	{
		if (objInt().PickedUp==true)
		{
			if (GameWorldController.instance.playerUW.playerInventory.ObjectInHand=="")
			{
				BecomeObjectInHand();
				UWHUD.instance.MessageScroll.Set (StringController.instance.GetString(1,130));
				return true;
			}
			else
			{
				return ActivateByObject(GameWorldController.instance.playerUW.playerInventory.GetGameObjectInHand());
			}
		}
		else
		{
			objInt().FailMessage ();
			return false;
		}
	}
}
