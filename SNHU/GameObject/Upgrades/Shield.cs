/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 2:23 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Shield.
	/// </summary>
	public class Shield : Upgrade
	{
		private Entity shield;
		private Image shieldImg;
		
		public Shield()
		{
			//image = new Image(Library.GetTexture("assets/" +  + ".png"));
			image = Image.CreateCircle(3, FP.Color(0xFF0000));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				shieldImg = new Image(Library.GetTexture("assets/shield_active.png"));
				shieldImg.CenterOO();
				shieldImg.Alpha = 0.0f;
			
				shield = Parent.World.AddGraphic(shieldImg);
				
				var tween = new VarTween(null, Tween.ONESHOT);
				tween.Tween(shieldImg, "Alpha", 0.6f, 0.45f);
				Parent.AddTween(tween, true);
				
				(Parent as Player).Invincible = true;
				
				Mixer.Audio["shieldUp"].Play();
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			FP.Log("SSQWE@#!@#!@#");
			(Parent as Player).Invincible = false;
			Parent.World.Remove(shield);
		}
		
		public override void Update()
		{
			base.Update();
			
			if (shieldImg != null)
			{
				shieldImg.Alpha = 0.6f - (0.6f * lifeTimer.Percent);
				shieldImg.X = Parent.X;
				shieldImg.Y = Parent.Y - Parent.HalfHeight;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			var tween = new VarTween(OnFadeOutComplete, Tween.ONESHOT);
			tween.Tween(shieldImg, "Alpha", 0.0f, 0.45f);
			Parent.AddTween(tween, true);
			
		}
		
		public void OnFadeOutComplete()
		{
			(Parent as Player).Invincible = false;
			Mixer.Audio["shieldDown"].Play();
			
			(Parent as Player).SetUpgrade(null);
		}
	}
}
