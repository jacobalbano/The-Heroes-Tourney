
using System;
using Punk;
using Punk.Graphics;
using Punk.Utils;
using SNHU.Components;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Victory.
	/// </summary>
	public class Victory : Entity
	{
		Image image;
		float ticks;
		private HypeTween hypeTween;
		
		public Victory(int layer)
		{
			Layer = layer;
			X = FP.Camera.X;
			Y = FP.Camera.Y;
			
			image = new Image(Library.GetTexture("assets/happy.png"));
			image.CenterOO();
			AddComponent(image);
			
			hypeTween = new HypeTween(1, Tweener);
			Tweener.Tween(image, new { Angle = 359 }, 20)
				.Repeat();
			
			ticks = 0;
			
		}
		
		
		public override void Update()
		{
			base.Update();
			
			image.Scale = 0.7f + (float) (Math.Sin(ticks += 0.1f) + 1) / 2f;
			image.Color = hypeTween.Color;
		}
	}
}
