﻿using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Shield.
	/// </summary>
	public class Shield : Upgrade
	{
		public const string SET = "shield_set";
		
		private Image shieldImg;
		
		public Shield()
		{
			Icon = new Image(Library.GetTexture("assets/shield.png"));
		}
		
		public override EffectMessage MakeEffect()
		{
			throw new NotImplementedException();
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				shieldImg = new Image(Library.GetTexture("assets/shield_active.png"));
				shieldImg.CenterOO();
				shieldImg.Alpha = 0.0f;
				
				shieldImg.Y = 10 - (shieldImg.Height / 2);
			
				Parent.AddGraphic(shieldImg);
				
				var tween = new VarTween(null, Tween.ONESHOT);
				tween.Tween(shieldImg, "Alpha", 0.6f, 0.45f);
				AddTween(tween, true);
				
				owner.OnMessage(Shield.SET, true);
				
				Mixer.Audio["shieldUp"].Play();
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			owner.OnMessage(Shield.SET, false);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (shieldImg != null)
			{
				shieldImg.Alpha = 0.6f - (0.6f * lifeTimer.Percent);
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			var tween = new VarTween(OnFadeOutComplete, Tween.ONESHOT);
			tween.Tween(shieldImg, "Alpha", 0.0f, 0.45f);
			AddTween(tween, true);
		}
		
		public void OnFadeOutComplete()
		{
			Mixer.Audio["shieldDown"].Play();
			
			owner.SetUpgrade(null);
		}
	}
}
