﻿using System;
using Punk;
using Punk.Graphics;
using Punk.Loaders;

namespace SNHU.GameObject.Platforms
{
	[OgmoConstructor("width", "height")]
	public class Crumble : Entity
	{
		private Nineslice image;
		private float crumbleTime = 0;
		private bool canMakeAHellaRacket;
		
		public Crumble(int width, int height)
		{
			Width = width;
			Height = height;
			Type = Platform.Collision;
			
			canMakeAHellaRacket = true;
			
			image = new Nineslice(Library.GetTexture("assets/crumble.png"), (int) (Width / 5f), (int) (Height / 5f));
			image.ScaleX = Width / image.Width;
			image.ScaleY = Height / image.Height;
			
			AddComponent(image);
			
			AddResponse(Player.Message.OnLand, OnBump);
			AddResponse(Platform.Message.Bump, OnBump);
		}
		
		
		public void OnBump(params object[] args)
        {
			if (canMakeAHellaRacket)
			{
				Tweener.Timer(crumbleTime).OnComplete(OnCrumble);
				canMakeAHellaRacket = false;
			}
        }
		
		public void OnCrumble()
		{
			Collidable = false;
			
			//Anim here
			image.OriginX = image.ScaledWidth / 2f;
			image.OriginY = image.ScaledHeight / 2f;
			X += image.OriginX;
			Y += image.OriginY;
			CenterOrigin();
			
			Tweener.Tween(image, new { Scale = 0, Alpha = 0, Y = image.Y + 200}, 1);
		}
	}
}
