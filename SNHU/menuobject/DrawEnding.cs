
using System;
using Glide;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using SNHU.Config;

namespace SNHU.MenuObject
{
	public class DrawEnding : Entity
	{
		public DrawEnding()
		{
			X = Engine.World.Camera.X;
			Y = Engine.World.Camera.Y;
			RenderStep = ObjectLayers.JustAbove(ObjectLayers.Background);
			
			var bg = AddComponent(Image.CreateRect(Engine.Width, Engine.Height, new Color()));
          	bg.CenterOrigin();

			var text = AddComponent(new Text("Draw!"));
			text.Font = Library.Get<Font>("fonts/Laffayette_Comic_Pro.ttf");
			text.Italicized = true;
			text.Y = -(Engine.Height / 4);
			text.Size = 64;
			text.CenterOrigin();
		}
	}
}
