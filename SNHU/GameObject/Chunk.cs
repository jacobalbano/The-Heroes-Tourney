using System;
using System.Collections.Generic;
using System.Linq;
using Punk;
using Punk.Tweens.Misc;
using Punk.Utils;
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
		public List<Entity> SpawnPoints;
		private Entity[]  ents;
		private string level;
		
		private static string[] levels;
		
		static Chunk()
		{
			levels = new string[]
			{
				"chris1.oel",
				"chris2.oel",
				"chris3.oel",
				"chris4.oel",
				"jake_1.oel",
				"Real_1.oel",
				"Real_2.oel",
				"Real_3.oel",
				"Start.oel"
			};
		}
		
		public Chunk(float posX, float posY) : base(posX, posY)
		{
			var world = new World();
			SpawnPoints = new List<Entity>();
			world.RegisterClass<Platform>("platform");
			world.RegisterClass<JumpPad>("jumpPad");
			world.RegisterClass<Crumble>("crumble");
			world.RegisterClass<Razor>("deadlyAnchor");
			world.RegisterClass<SpawnPoint>("spawnPoint");
			world.RegisterClass<UpgradeSpawn>("Upgrade");
			
			level = FP.Choose(levels);
			
			var t = FP.GetTimer();
			ents = world.BuildWorldAsArray("assets/Levels/" + level);
			FP.Log("building", level, "took", FP.GetTimer() - t, "ms");
			
			int spawns = 0;
			foreach (var e in ents)
			{
				if (!(e is Player))
				{
					e.X += X;
					e.Y += Y;
					e.Layer = -100;
				}
				
				if (e is SpawnPoint)
				{
					++spawns;
					SpawnPoints.Add(e);
				}
			}
			
			if (spawns != 4)
			{
				throw new Exception("too few spawn points in" + level + ";" + spawns + "found, 4 required.");
			}
		}
		
		
		public override void Added()
		{
			base.Added();
			
			World.AddList(ents);
			
			var tween = new VarTween(OnFinishAdvance, ONESHOT);
			tween.Tween(FP.Camera, "Y", Y + FP.HalfHeight, 1, Ease.ElasticOut);
			AddTween(tween, true);
		}
		
		public override void Removed()
		{
			base.Removed();
			World.RemoveList(ents);
		}
		
		public bool IsBelowCamera
		{
			get { return (FP.Camera.Y + (FP.Height / 2)) < this.Y; }
		}
		
		private void OnFinishAdvance()
		{
			World.Add(new CameraShake(FP.HalfWidth, FP.Camera.Y));
			World.BroadcastMessage(ChunkManager.SpawnPlayers);
		}
	}
}
