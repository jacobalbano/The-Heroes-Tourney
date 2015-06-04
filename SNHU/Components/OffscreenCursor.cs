using System;
using Indigo;
using Indigo.Graphics;
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
			cursor.Layer = ObjectLayers.JustBelow(ObjectLayers.HUD);
			image = cursor.AddComponent(new Image(Library.GetTexture("cursor.png")));
			image.CenterOrigin();
			image.OriginX = image.Width * 0.35f;
			
			var face = cursor.AddComponent(new Image(Library.GetTexture("players/" + ImageName + "_head.png")));
			face.CenterOO();
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
		
		public override void Update()
		{
			base.Update();
			
			cursor.Visible = Visible = Parent.World != null && !Parent.OnCamera;
			
			cursor.X = Parent.X - Parent.HalfWidth;
			cursor.Y = Parent.Y - Parent.HalfHeight;
			FP.ClampInRect(ref cursor.X, ref cursor.Y, FP.Camera.X - FP.HalfWidth, FP.Camera.Y - FP.HalfHeight, FP.Width, FP.Height, 25);
			
			image.Angle = FP.Angle(FP.Camera.X, FP.Camera.Y, cursor.X, cursor.Y);
		}
	}
}
