/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/15/2013
 * Time: 7:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using SNHU.Components;

namespace SNHU.GameObject.Platforms
{
	public class Platform : Entity
	{
		public const string Collision = "platform";
		public const string NotifyCamera = "platform_cameraNotification";
		public const string PlayerLand = "player_land";
		
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
			
			AddResponse(NotifyCamera, OnNotifyCamera);
			AddResponse(PlayerLand, OnPlayerLandResponse);
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
			
			Graphic = image;
		}
		
		private void OnNotifyCamera(params object[] args)
		{
			World.Remove(this);
		}

        public virtual void OnLand()
        {
        	
        }
	}
}
