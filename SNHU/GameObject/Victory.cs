
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
		private ColorTween colorTween;
		
		static int[] colors;
		static Victory()
		{
			colors = new int[]
			{
				0xff0000,
				0x00ff00,
				0x0000ff,
				0xff00ff,
				0x00ffff,
				0xffaa00,
				0x00ffaa,
				0xff00aa,
				0xaaff00,
				0x00aaff
			};
		}
		
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
			
			colorTween = new ColorTween(NewColorPls, LOOPING);
			NewColorPls();
			AddTween(colorTween, true);
		}
		
		void NewColorPls()
		{
			colorTween.Tween(1, colorTween.Color, FP.Color(FP.Choose(colors)));
		}
		
		public override void Update()
		{
			base.Update();
			
			image.Scale = 0.7f + (float) (Math.Sin(ticks += 0.1f) + 1) / 2f;
			image.Color = colorTween.Color;
		}
	}
}
