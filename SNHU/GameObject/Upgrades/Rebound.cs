using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using SNHU.Components;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Rebound.
	/// </summary>
	public class Rebound : Upgrade
	{
		public const string SET = "rebound_set";
		
		private Image shield_1, shield_2;
		
		public Rebound()
		{
			Icon = new Image(Library.GetTexture("assets/rebound.png"));
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
				
				owner.OnMessage(Rebound.SET, true);
				
				Mixer.ReboundUp.Play();
				
				var existingCollisions = new List<Entity>();
				Parent.CollideInto(Parent.Type, Parent.X, Parent.Y, existingCollisions);
				foreach (var player in existingCollisions)
				{
					player.OnMessage(PhysicsBody.IMPULSE, 0, -Fist.BASE_PUNCH_FORCE);
				}
			}
		}
		
		public override void Removed()
		{
			base.Removed();
			owner.OnMessage(Rebound.SET, false);
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
			Mixer.ReboundDown.Play();	
			owner.SetUpgrade(null);
		}
	}
}
