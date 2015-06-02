
using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using SNHU.Config;

namespace SNHU.MenuObject
{
	public class DrawEnding : Entity
	{
		public DrawEnding()
		{
			X = FP.Camera.X;
			Y = FP.Camera.Y;
			Layer = ObjectLayers.JustAbove(ObjectLayers.Background);
			
			var bg = AddComponent(Image.CreateRect(FP.Width, FP.Height, new Color()));
          	bg.CenterOO();

			var text = AddComponent(new Text("Draw!"));
			text.Font = Library.GetFont("assets/fonts/Laffayette_Comic_Pro.ttf");
			text.Italicized = true;
			text.Y = -(FP.Height / 4);
			text.Size = 64;
			text.CenterOrigin();
		}
	}
}
