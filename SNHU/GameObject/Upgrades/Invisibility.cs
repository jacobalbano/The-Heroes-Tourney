using System;
using Indigo;
using Indigo.Graphics;
using SNHU.Config.Upgrades;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Invisibility.
	/// </summary>
	public class Invisibility : Upgrade
	{
		public float InvisibleAlpha { get; private set; }
		public float PunchMultiplier { get; private set; }
		
		public Invisibility()
		{
			Icon = new Image(Library.GetTexture("invisibility.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			var config = Library.GetConfig<InvisibilityConfig>("config/upgrades/invisibility.ini");
			InvisibleAlpha = config.Alpha;
			PunchMultiplier = config.PunchMultiplier;
			Lifetime = config.Lifetime;
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
			
				Tweener.Tween(owner.Fists, new { Alpha = InvisibleAlpha }, 0.25f);
				Tweener.Tween(owner.player, new { Alpha = InvisibleAlpha }, 0.25f);
				
				owner.Fists.ForceMultiplier = PunchMultiplier;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			owner.Fists.ForceMultiplier = Fist.DEFAULT_PUNCH_MULT;
			
			Tweener.Tween(owner.Fists, new { Alpha = 1 }, 0.25f);
			Tweener.Tween(owner.player, new { Alpha = 1 }, 0.25f)
				.OnComplete(() => owner.SetUpgrade(null));
		}
	}
}
