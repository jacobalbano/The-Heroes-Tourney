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
using SNHU.Actor;

namespace SNHU.GameObject
{
	public class Platform : Entity
	{
		private Player owner;
		public Player myOwner
		{
			set
			{
				owner = value;
			}
		}
		private Image myImage;
		
		public Platform(float posX, float posY, uint width, uint height):base(posX, posY)
		{
			Graphic = myImage = Image.CreateRect(width, height, FP.Color(0x00FF00));
			SetHitboxTo(Graphic);
			
			myImage.CenterOO();
			CenterOrigin();
		}

        public virtual void OnLand(Player playerTarget)
        {
        	FP.Log("Landed");
        }

        public virtual void OnLeave(Player playerTarget)
        {
        	FP.Log("Left");
        }
	}
}
