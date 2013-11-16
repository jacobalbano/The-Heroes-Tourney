﻿/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 12:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Platforms
{
	public class Crumble : Platform
	{
		private float crumbleTime;
		
		public Crumble()
		{
			
		}
		
		public override void Update()
		{
			base.Update();
			
			var p = Collide(Player.Collision, X - 1, Y) as Player;
			if (p == null)
			{
				p = Collide(Player.Collision, X + 1, Y) as Player;
			}
			
			if (p != null)
			{
				OnLand(p);
			}
		}
		
		public override void OnLand(Player playerTarget)
        {
			AddTween(new Alarm(crumbleTime, OnCrumble, ONESHOT), true);
        }
		
		public void OnCrumble()
		{
			//Anim here
			image.OriginX = image.ScaledWidth / 2f;
			image.OriginY = image.ScaledHeight / 2f;
			X += image.OriginX;
			Y += image.OriginY;
			CenterOrigin();
			
			var tween = new MultiVarTween(null, ONESHOT);
			tween.Tween(image, new { Scale = 0, Alpha = 0, Y = image.Y + 200}, 1);
			AddTween(tween, true);
			
			Collidable = false;
			
			Mixer.Audio["crumble"].Play();
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			crumbleTime = uint.Parse(node.Attributes["crumbleTime"].Value);
			
			float width = float.Parse(node.Attributes["width"].Value);
			float height = float.Parse(node.Attributes["height"].Value);
			
			image = new Nineslice(Library.GetTexture("assets/crumble.png"), 3, 3);
			image.Columns = (uint) (width / 5f);
			image.Rows = (uint) (height / 5f);
			image.ScaleX = width / image.Width;
			image.ScaleY = height / image.Height;
			
			Graphic = image;
			SetHitbox((int) width, (int) height);
		}
	}
}
