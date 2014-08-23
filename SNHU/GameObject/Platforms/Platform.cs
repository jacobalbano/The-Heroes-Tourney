using System;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	[OgmoConstructor("width", "height")]
	public class Platform : Entity
	{
		public const string Collision = "Collision";
		
		public enum Message { Bump }
		
		private Nineslice image;
		
		public Platform(int width, int height)
		{
			Width = width;
			Height = height;
			
			Type = Collision;
			
			image = new Nineslice(Library.GetTexture("assets/platform.png"), 3, 3);
			image.Columns = (int) (Width / 5f);
			image.Rows = (int) (Height / 5f);
			image.ScaleX = Width / image.Width;
			image.ScaleY = Height / image.Height;
			
			AddComponent(image);
		}
	}
}
