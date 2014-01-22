using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of FUS.
	/// </summary>
	public class FUS : Upgrade
	{
		public const string BE_FUS = "fusYou";
		const float FUS_STRENGTH = 65.0f;
			
		public FUS()
		{
			Icon = new Image(Library.GetTexture("assets/fus.png"));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				if (Parent.World != null)
				{
					Parent.World.BroadcastMessage(BE_FUS, FUS_STRENGTH, Parent);
					Parent.World.BroadcastMessage(CameraShake.SHAKE, 60.0f, 0.5f);
					owner.SetUpgrade(null);
					Mixer.Audio["fus"].Play();
					
					Parent.World.Add(new FusBlast(Parent.X, Parent.Y));
				}
			}
		}
		
		private class FusBlast : Entity
		{
			public FusBlast(float X, float Y)
			{
				this.X = X;
				this.Y = Y;
				
				var i = new Image(Library.GetTexture("assets/fus_active.png"));
				i.Scale = 0.1f;
				i.CenterOO();
				
				Graphic = i;
				
				var tween = new VarTween(() => World.Remove(this), ONESHOT);
				tween.Tween(i, "Scale", 50, 0.5f);
				AddTween(tween, true);
			}
		}
	}
}
