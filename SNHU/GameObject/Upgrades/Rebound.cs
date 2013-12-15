using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Rebound.
	/// </summary>
	public class Rebound : Upgrade
	{
		private Image shield_1, shield_2;
		
		public Rebound()
		{
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				shield_1 = new Image(Library.GetTexture("assets/rebound_active_1.png"));
				shield_2 = new Image(Library.GetTexture("assets/rebound_active_2.png"));
				
				shield_1.CenterOO();
				shield_2.CenterOO();
				
				shield_1.Alpha = 0.0f;
				shield_2.Alpha = 0.0f;
			
				Parent.AddGraphic(shield_1);
				Parent.AddGraphic(shield_2);
				
				shield_1.Y = 10 - (shield_1.Height / 2);
				shield_2.Y = 10 - (shield_2.Height / 2);
				
				var tween1 = new VarTween(null, Tween.ONESHOT);
				tween1.Tween(shield_1, "Alpha", 0.6f, 0.45f);
				AddTween(tween1, true);
				
				var tween2 = new VarTween(null, Tween.ONESHOT);
				tween2.Tween(shield_2, "Alpha", 0.6f, 0.45f);
				AddTween(tween2, true);
				
				owner.Rebounding = true;
				
				Mixer.Audio["reboundUp"].Play();
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			
			owner.Rebounding = false;
		}
		
		public override void Update()
		{
			base.Update();
			
			if (shield_1 != null)
			{
				shield_1.Alpha = 0.6f - (0.6f * lifeTimer.Percent);
				shield_2.Alpha = 0.6f - (0.6f * lifeTimer.Percent);
				
				shield_1.Angle++;
				shield_2.Angle--;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			var tween1 = new VarTween(OnFadeOutComplete, Tween.ONESHOT);
			tween1.Tween(shield_1, "Alpha", 0.0f, 0.45f);
			AddTween(tween1, true);
			
			var tween2 = new VarTween(OnFadeOutComplete, Tween.ONESHOT);
			tween2.Tween(shield_2, "Alpha", 0.0f, 0.45f);
			AddTween(tween2, true);
		}
		
		public void OnFadeOutComplete()
		{
			owner.Rebounding = false;
				Mixer.Audio["reboundDown"].Play();
			
			owner.SetUpgrade(null);
		}
	}
}
