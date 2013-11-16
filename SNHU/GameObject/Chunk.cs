﻿/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/15/2013
 * Time: 9:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject
{
	/// <summary>
	/// A section of the map that contains platforms
	/// </summary>
	public class Chunk : Entity
	{
		public const uint CHUNK_WIDTH = 640;
		public const uint CHUNK_HEIGHT = 640;
		
		
		private Entity[] ents;
		
		public Chunk(float posX, float posY):base(posX,posY)
		{
			var world = new World();
			world.RegisterClass<Platform>("platform");
			world.RegisterClass<JumpPad>("jumpPad");
			world.RegisterClass<Crumble>("crumble");
			world.RegisterClass<Teleporter>("teleporter");
			
			ents = world.BuildWorldAsArray("assets/Levels/Test2.oel");
			FP.Log("load");
		}
		
		
		public override void Added()
		{
			base.Added();
			FP.Log("add");
			
			foreach (var e in ents)
			{
				e.X += X;
				e.Y += Y;
				e.Layer = -100;
				
				if (e is Teleporter)
				{
					(e as Teleporter).ID += (uint)Math.Abs(Y);
				}
			}
			
			World.AddList(ents);
		}
		
		public override void Removed()
		{
			World.RemoveList(ents);
			
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
			
			if (IsBelowCamera && World != null)
			{
				//World.Remove(this);
			}
		}
		
		public bool IsBelowCamera
		{
			get { return (FP.Camera.Y + (FP.Height / 2)) < this.Y; }
		}
	}
}
