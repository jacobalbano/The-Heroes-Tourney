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
using Punk.Tweens.Misc;
using Punk.Graphics;

namespace SNHU.GameObject.Platforms
{
	public class Crumble : Platform
	{
		private float crumbleTime;
		
		public Crumble()
		{
			
		}
		
		public override void OnLand(Player playerTarget)
        {
			AddTween(new Alarm(crumbleTime, OnCrumble, ONESHOT), true);
        }
		
		public void OnCrumble()
		{
			//Anim here
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			uint height = uint.Parse(node.Attributes["height"].Value);
			crumbleTime = uint.Parse(node.Attributes["crumbleTime"].Value);
			
			Graphic = myImage = Image.CreateRect(width, height, FP.Color(0x00FF00));
			SetHitboxTo(Graphic);
		}
	}
}
