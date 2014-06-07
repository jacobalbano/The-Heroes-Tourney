using System;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	public class Platform : Entity
	{
		public const string Collision = "platform";
		
		public enum Message { ObjectCollide }
		
		private Player owner;
		
		public Player myOwner
		{
			set
			{
				owner = value;
			}
		}
		public Nineslice image;
		
		public Platform():base()
		{
			Type = Collision;
			
			AddResponse(Message.ObjectCollide, OnPlayerLandResponse);
		}
		
		public void OnPlayerLandResponse(params object[]args)
		{
			OnLand();
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
			image = new Nineslice(Library.GetTexture("assets/platform.png"), 3, 3);
			image.Columns = (int) (Width / 5f);
			image.Rows = (int) (Height / 5f);
			image.ScaleX = Width / image.Width;
			image.ScaleY = Height / image.Height;
			
			AddComponent(image);
		}
		
        public virtual void OnLand()
        {
        	
        }
	}
}
