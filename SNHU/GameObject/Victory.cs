
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using Punk.Utils;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Victory.
	/// </summary>
	public class Victory : Entity
	{
		Image image;
		float ticks;
		
		public Victory(int layer)
		{
			Layer = layer;
			X = FP.Camera.X;
			Y = FP.Camera.Y;
			
			image = new Image(Library.GetTexture("assets/happy.png"));
			image.CenterOO();
			Graphic = image;
			
			var angle = new VarTween(() => image.Angle = 0, LOOPING);
			angle.Tween(image, "Angle", 359, 20);
			AddTween(angle, true);
			
			ticks = 0;
		}
		
		public override void Update()
		{
			base.Update();
			
			image.Scale = 0.7f + (float) (Math.Sin(ticks += 0.1f) + 1) / 2f;
		}
	}
}
