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
		public Image myImage;
		
		public Platform():base()
		{
			Type = Collision;
			
			AddResponse(NotifyCamera, OnNotifyCamera);
			AddResponse(PlayerLand, OnPlayerLandResponse);
		}
		
		public void OnPlayerLandResponse(params object[]args)
		{
			OnLand(args[0] as Player);
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			
			Graphic = myImage = Image.CreateRect(width, 16, FP.Color(0x00FF00));
			SetHitboxTo(Graphic);
		}
		
		private void OnNotifyCamera(params object[] args)
		{
			World.Remove(this);
		}

        public virtual void OnLand(Player playerTarget)
        {
        	
        }

        public virtual void OnLeave(Player playerTarget)
        {
        }
	}
}
