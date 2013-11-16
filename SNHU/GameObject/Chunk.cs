/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/15/2013
 * Time: 9:07 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
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
		public List<SpawnPoint> spawnPointsList;
		private Entity[] ents;
		
		public Chunk(float posX, float posY, string chunkType = "random"):base(posX,posY)
		{
			var world = new World();
			spawnPointsList = new List<SpawnPoint>();
			world.RegisterClass<Platform>("platform");
			world.RegisterClass<JumpPad>("jumpPad");
			world.RegisterClass<Crumble>("crumble");
			world.RegisterClass<Razor>("deadlyAnchor");
			world.RegisterClass<SpawnPoint>("spawnPoint");
			
			var t = FP.GetTimer();
			
//			switch (chunkType)
//			{
//				case "start":
//					ents = world.BuildWorldAsArray("assets/Levels/Start.oel");
//					break;
//				case "end":
//					ents = world.BuildWorldAsArray("assets/Levels/End.oel");
//					break;
//				case "random":
//					ents = world.BuildWorldAsArray("assets/Levels/" + FP.Choose("chris1.oel", "chris2.oel", "chris3.oel"));
//					break;
//				default:
					ents = world.BuildWorldAsArray("assets/Levels/Test.oel");
//					break;
//			}
//			
			FP.Log(FP.GetTimer() - t);
		}
		
		
		public override void Added()
		{
			base.Added();
			
			foreach (var e in ents)
			{
				e.X += X;
				e.Y += Y;
				e.Layer = -100;
				
				if (e is Teleporter)
				{
					(e as Teleporter).ID += (int)Math.Abs(Y);
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
		}
		
		public bool IsBelowCamera
		{
			get { return (FP.Camera.Y + (FP.Height / 2)) < this.Y; }
		}
	}
}
