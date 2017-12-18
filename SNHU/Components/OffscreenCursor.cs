using System;
using Indigo;
using Indigo.Content;
using Indigo.Graphics;
using Indigo.Utils;
using SNHU.Config;
using SNHU.GameObject;

namespace SNHU.Components
{
	public class OffscreenCursor : Component
	{
		Entity cursor;
		Image image;
		
		public OffscreenCursor(string ImageName)
		{
			cursor = new Entity();
			cursor.RenderStep = ObjectLayers.JustBelow(ObjectLayers.HUD);
			image = cursor.AddComponent(new Image(Library.Get<Texture>("cursor.png")));
			image.CenterOrigin();
			image.OriginX = image.Width * 0.35f;
			
			var face = cursor.AddComponent(new Image(Library.Get<Texture>("players/" + ImageName + "_head.png")));
			face.CenterOrigin();
		}
		
		public override void ParentAdded()
		{
			base.ParentAdded();
			Parent.World.Add(cursor);
		}
		
		public override void ParentRemoved()
		{
			base.ParentRemoved();
			Parent.World.Remove(cursor);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			cursor.Visible = Visible = Parent.World != null && !Parent.OnCamera;
			
			cursor.X = Parent.X - Parent.HalfWidth;
			cursor.Y = Parent.Y - Parent.HalfHeight;
			MathHelper.ClampInRect(ref cursor.X, ref cursor.Y, Engine.World.Camera.X - Engine.HalfWidth, Engine.World.Camera.Y - Engine.HalfHeight, Engine.Width, Engine.Height, 25);
			
			image.Angle = MathHelper.Angle(Engine.World.Camera.X, Engine.World.Camera.Y, cursor.X, cursor.Y);
		}
	}
}
