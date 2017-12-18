using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using SNHU.Components;
using SNHU.Config.Upgrades;
using SNHU.Systems;

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
			Icon = new Image(Library.Get<Texture>("shield.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			Lifetime = Library.Get<ShieldConfig>("config/upgrades/shield.ini").Lifetime;
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
				
				shieldImg = new Image(Library.Get<Texture>("shield_active.png"));
				shieldImg.CenterOrigin();
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
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
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
