/*
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
			myImage.CenterOO();
			X += myImage.OriginX;
			Y += myImage.OriginY;
			CenterOrigin();
			
			var tween = new MultiVarTween(null, ONESHOT);
			tween.Tween(myImage, new { Scale = 0, Alpha = 0, Y = myImage.Y + 200}, 1);
			AddTween(tween, true);
			
			Collidable = false;
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			crumbleTime = uint.Parse(node.Attributes["crumbleTime"].Value);
			
			Graphic = myImage = Image.CreateRect(width, 32, FP.Color(0xFF0000));
			SetHitboxTo(Graphic);
		}
	}
}
