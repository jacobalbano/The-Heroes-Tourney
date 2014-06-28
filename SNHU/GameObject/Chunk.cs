using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Glide;
using Punk;
using Punk.Loaders;
using Punk.Utils;
using SNHU.GameObject.Platforms;

namespace SNHU.GameObject
{
	/// <summary>
	/// A section of the map that contains platforms
	/// </summary>
	public class Chunk : Entity
	{
		public List<Entity> SpawnPoints;
		private Entity[] ents;
		private string level;
		
		private static string[] levels;
		
		static Chunk()
		{
			levels = Directory.GetFiles("assets/Levels/");
			for (int i = 0; i < levels.Length; i++)
			{
				var level = levels[i];
				level = level.Substring(level.LastIndexOf("/"));
				levels[i] = level;
			}
			
			var l = levels.ToList();
			l.Remove("/bawks.oel");
			levels = l.ToArray();
		}
		
		public Chunk(float posX, float posY) : base(posX, posY)
		{
			var world = new World();
			SpawnPoints = new List<Entity>();
			
			level = FP.Choose(levels);
			
			var loader = new OgmoLoader();
			loader.RegisterClassAlias<Platform>("platform");
			loader.RegisterClassAlias<JumpPad>("jumpPad");
			loader.RegisterClassAlias<Crumble>("crumble");
			loader.RegisterClassAlias<Razor>("deadlyAnchor");
			loader.RegisterClassAlias<SpawnPoint>("spawnPoint");
			loader.RegisterClassAlias<UpgradeSpawn>("Upgrade");
			
			ents = loader.BuildWorldAsArray(Library.GetXml("assets/Levels/" + level));
			
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
			
			Tweener.Tween(FP.Camera, new { Y = Y + FP.HalfHeight}, 1)
				.Ease(Ease.ElasticOut)
				.OnComplete(OnFinishAdvance);
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
			World.BroadcastMessage(ChunkManager.Message.SpawnPlayers);
		}
	}
}
