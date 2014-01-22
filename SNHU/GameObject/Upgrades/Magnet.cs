using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	public class Magnet : Upgrade
	{
		public const string BE_MAGNET = "GET OVER HERE";
		const float MAGNET_STRENGTH = 65.0f;
			
		public Magnet()
		{
			Icon = new Image(Library.GetTexture("assets/magnet.png"));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
				
				if (Parent.World != null)
				{
					Parent.World.BroadcastMessage(BE_MAGNET, MAGNET_STRENGTH, Parent);
					Parent.World.BroadcastMessage(CameraShake.SHAKE, 60.0f, 0.5f);
					owner.SetUpgrade(null);
					Mixer.Audio["fus"].Play();
					
//					Parent.World.Add(new FusBlast(Parent.X, Parent.Y));
				}
			}
		}
	}
}
