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
		public const string LOAD_COMPLETE = "loadComplete";
		public const uint CHUNK_WIDTH = 640;
		public const uint CHUNK_HEIGHT = 640;
		public List<Entity> spawnPoints;
		private Entity[] ents;
		
		public Chunk(float posX, float posY) : base(posX, posY)
		{
			var world = new World();
			spawnPoints = new List<Entity>();
			world.RegisterClass<Platform>("platform");
			world.RegisterClass<JumpPad>("jumpPad");
			world.RegisterClass<Crumble>("crumble");
			world.RegisterClass<Razor>("deadlyAnchor");
			world.RegisterClass<SpawnPoint>("spawnPoint");
			world.RegisterClass<UpgradeSpawn>("Upgrade");
			
			var t = FP.GetTimer();
			
			ents = world.BuildWorldAsArray("assets/Levels/" +
            FP.Choose("chris1.oel", "chris2.oel", "chris3.oel", "chris4.oel",
			          "jake_1.oel", "Real_1.oel", "Real_2.oel", "Real_3.oel",
			          "Start.oel"));
			
			FP.Log(FP.GetTimer() - t);
		}
		
		
		public override void Added()
		{
			base.Added();
			
			foreach (var e in ents)
			{
				if (!(e is Player))
				{
					e.X += X;
					e.Y += Y;
					e.Layer = -100;
				}
				
				if (e is Teleporter)
				{
					(e as Teleporter).ID += (int)Math.Abs(Y);
				}
				
				if (e is SpawnPoint)
				{
					spawnPoints.Add(e);
				}
			}
			
			World.AddList(ents);
			
			if (World is GameWorld)
			{
				(World as GameWorld).ChunkLoadComplete();
			}
		}
		
		public override void Removed()
		{
			World.RemoveList(ents);
			
			base.Removed();
		}
		
		public bool IsBelowCamera
		{
			get { return (FP.Camera.Y + (FP.Height / 2)) < this.Y; }
		}
	}
}
