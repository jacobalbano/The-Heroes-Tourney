using System;
using Punk;
using Punk.Graphics;
using SNHU.GameObject;

namespace SNHU
{
	/// <summary>
	/// Description of OffscreenCursor.
	/// </summary>
	public class OffscreenCursor : Entity
	{
		Entity target;
		Image image;
		
		public OffscreenCursor(Player target)
		{
			this.target = target;
			
			image = new Image(Library.GetTexture("assets/cursor.png"));
			image.CenterOrigin();
			image.OriginX = image.Width * 0.35f;
			AddGraphic(image);
			
			var face = new Image(Library.GetTexture("assets/players/" + target.ImageName + "_head.png"));
			face.CenterOO();
			AddGraphic(face);
		}
		
		public override void Added()
		{
			base.Added();
			
			Layer = -1000;
		}
		
		public override void Update()
		{
			base.Update();
			
			X = target.X - target.HalfWidth;
			Y = target.Y - target.HalfHeight;
			FP.ClampInRect(ref X, ref Y, FP.Camera.X - FP.HalfWidth, FP.Camera.Y - FP.HalfHeight, FP.Width, FP.Height, 25);
			
			image.Angle = FP.Angle(FP.Camera.X, FP.Camera.Y, X, Y);
		}
	}
}
