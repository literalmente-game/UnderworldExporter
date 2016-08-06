﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Freezes the NPC
/// </summary>
public class SpellEffectParalyze : SpellEffect {
		public bool isNPC;//The effect is applied to an npc
		public NPC npc;//THe NPC the spell is applied to.
		///The state the npc was in before the spell was cast at them
		public int state;
		public Animator anim;

		public override void ApplyEffect ()
		{
			if (isNPC==false)
			{//Apply to player
					if (GameWorldController.instance.playerUW==null)
					{
							GameWorldController.instance.playerUW= this.GetComponent<UWCharacter>();
					}
					GameWorldController.instance.playerUW.Paralyzed=true;
			}
			else
			{
				if (npc==null)
				{
						npc=this.GetComponent<NPC>();
				}
				this.GetComponent<NPC>().Frozen=true;
				state = this.GetComponent<NPC>().state;
				anim = this.GetComponent<NPC>().anim;
				if (anim!=null)
				{
						anim.enabled=false;
				}
			}
			base.ApplyEffect();
		}

		public override void CancelEffect ()
		{
			if (isNPC==false)
			{
				GameWorldController.instance.playerUW.Paralyzed=false;
				GameWorldController.instance.playerUW.walkSpeed=3.0f;
			}
			else
			{
				npc.Frozen=false;
				npc.CurrentAnim="";
				npc.currentState=-1;
				npc.state=state;
				if (anim!=null)
				{
					anim.enabled=true;
				}
			}
			base.CancelEffect();
		}

		public void Update()
		{//Maintain the effect
			if (isNPC==true)
			{
					npc.Frozen=true;
			}
			else
			{
					GameWorldController.instance.playerUW.Paralyzed=true;
			}
		}

}
