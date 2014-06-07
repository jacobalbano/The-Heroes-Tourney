using System;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject.Platforms
{
	public class Crumble : Platform
	{
		private float crumbleTime = 0;
		private bool canMakeAHellaRacket;
		
		public Crumble()
		{
			canMakeAHellaRacket = true;
		}
		
		public override void Update()
		{
			base.Update();
			
			var p = Collide(Player.Collision, X - 1, Y) as Player;
			if (p == null)
			{
				p = Collide(Player.Collision, X + 1, Y) as Player;
			}
			
			if(p == null)
			{
				p = Collide(Player.Collision, X, Y + 1) as Player;
			}
			
			if (p != null)
			{
				OnLand();
			}
		}
		
		public override void OnLand()
        {
			if (canMakeAHellaRacket)
			{
				Tweener.Timer(crumbleTime).OnComplete(OnCrumble);
				canMakeAHellaRacket = false;
			}
        }
		
		public void OnCrumble()
		{
			Collidable = false;
			
			//Anim here
			image.OriginX = image.ScaledWidth / 2f;
			image.OriginY = image.ScaledHeight / 2f;
			X += image.OriginX;
			Y += image.OriginY;
			CenterOrigin();
			
			Tweener.Tween(image, new { Scale = 0, Alpha = 0, Y = image.Y + 200}, 1);
		}
		
		public override void Load(System.Xml.XmlNode node)
		{	
			image = new Nineslice(Library.GetTexture("assets/crumble.png"), (int) (Width / 5f), (int) (Height / 5f));
			image.ScaleX = Width / image.Width;
			image.ScaleY = Height / image.Height;
			
			AddComponent(image);
		}
	}
}
