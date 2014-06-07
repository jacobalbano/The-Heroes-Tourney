using System;
using Punk;
using Punk.Graphics;

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
			Icon = new Image(Library.GetTexture("assets/invisibility.png"));
		}
		
		public override void Added()
		{
			base.Added();
			
			InvisibleAlpha = float.Parse(GameWorld.gameManager.Config["Invisibility", "InvisibleAlpha"]);
			PunchMultiplier = float.Parse(GameWorld.gameManager.Config["Invisibility", "PunchMultiplier"]);
			Lifetime = float.Parse(GameWorld.gameManager.Config["Invisibility", "Lifetime"]);
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
			
				Tweener.Tween(owner.left.Image, new { Alpha = InvisibleAlpha }, 0.25f);
				Tweener.Tween(owner.right.Image, new { Alpha = InvisibleAlpha }, 0.25f);
				Tweener.Tween(owner.player, new { Alpha = InvisibleAlpha }, 0.25f);
				
				owner.right.ForceMultiplier = owner.left.ForceMultiplier = PunchMultiplier;
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			owner.right.ForceMultiplier = owner.left.ForceMultiplier = Fist.DEFAULT_PUNCH_MULT;
			
			Tweener.Tween(owner.left.Image, new { Alpha = 1 }, 0.25f);
			Tweener.Tween(owner.right.Image, new { Alpha = 1 }, 0.25f);
			Tweener.Tween(owner.player, new { Alpha = 1 }, 0.25f)
				.OnComplete(() => owner.SetUpgrade(null));
		}
	}
}
