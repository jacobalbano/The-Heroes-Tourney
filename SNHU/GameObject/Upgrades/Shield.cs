﻿using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Shield.
	/// </summary>
	public class Shield : Upgrade
	{
		public new enum Message { Set }
		
		private Image shieldImg;
		
		public Shield()
		{
			Icon = new Image(Library.GetTexture("assets/shield.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			Lifetime = float.Parse(GameWorld.gameManager.Config["Shield", "Lifetime"]);
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
			
				Parent.AddComponent(shieldImg);
				Tweener.Tween(shieldImg, new { Alpha = 0.6f }, 0.45f);
				
				owner.OnMessage(Shield.Message.Set, true);
				
				Mixer.ShieldUp.Play();
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			owner.OnMessage(Shield.Message.Set, false);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (shieldImg != null)
			{
				shieldImg.Alpha = 0.6f - (0.6f * lifeTimer.Completion);
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			Tweener.Tween(shieldImg, new { Alpha = 0 }, 0.45f)
				.OnComplete(OnFadeOutComplete);
		}
		
		public void OnFadeOutComplete()
		{
			Mixer.ShieldDown.Play();
			
			owner.SetUpgrade(null);
		}
	}
}
