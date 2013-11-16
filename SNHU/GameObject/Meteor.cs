/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 6:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;
using SFML.Window;
using Punk.Utils;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Meteor.
	/// </summary>
	public class Meteor : Entity
	{
		private float targetX;
		private float targetY;
		
		Vector2f direction;
		List<Vector2f> SpawnPoints;
		
		public const int SPEED = 9;
		
		public Meteor()
		{
			Graphic = Image.CreateCircle(60, FP.Color(0xFF9900));
			//Graphic.ScrollX = Graphic.ScrollY = 0;
			
			float cX = FP.Camera.X - FP.Width;
			float cY = FP.Camera.Y - FP.Height;
			
			SpawnPoints = new List<Vector2f>();
			SpawnPoints.Add(new Vector2f(cX - 100, cY));
			SpawnPoints.Add(new Vector2f(cX - 100, cY + FP.HalfHeight));
			SpawnPoints.Add(new Vector2f(cX - 100, cY + FP.Height));
			SpawnPoints.Add(new Vector2f(cX + FP.Width + 100, cY));
			SpawnPoints.Add(new Vector2f(cX + FP.Width + 100, cY + FP.HalfHeight));
			SpawnPoints.Add(new Vector2f(cX + FP.Width + 100, cY + FP.Height));
			SpawnPoints.Add(new Vector2f(cX, cY - 100));
			SpawnPoints.Add(new Vector2f(cX + FP.HalfWidth, cY - 100));
			SpawnPoints.Add(new Vector2f(cX + FP.Width, cY - 100));
			SpawnPoints.Add(new Vector2f(cX, cY + FP.Height + 100));
			SpawnPoints.Add(new Vector2f(cX + FP.HalfWidth, cY + FP.Height + 100));
			SpawnPoints.Add(new Vector2f(cX + FP.Width, cY + FP.Height + 100));
			
			Vector2f v = FP.Choose(SpawnPoints.ToArray());
			X = v.X;
			Y = v.Y;
		}
		
		public override void Added()
		{
			base.Added();
			
			List<Entity> eList = new List<Entity>();
			
			World.GetType(Player.Collision, eList);
			
			Entity e = FP.Choose(eList.ToArray());
			
			targetX = e.X;
			targetY = e.Y;
			
			direction = new Vector2f(targetX - X, targetY - Y).Normalized(1.0f);
			
			World.AddTween(new Alarm(3.0f, OnComplete, Tween.ONESHOT));
		}
		
		public override void Update()
		{
			base.Update();
			
			MoveBy(direction.Normalized(SPEED).X, direction.Normalized(SPEED).Y);
		}
		
		public void OnComplete()
		{
			if (World != null)
			{
				World.Remove(this);
			}
		}
	}
}
