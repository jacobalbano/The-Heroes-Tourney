using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;
using Indigo.Masks;
using Indigo.Utils;
using Indigo.Utils.Reflect;
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
		private static OgmoLoader loader;
		
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
			
			levels = new string[] { "Real_3.oel" };
			
			loader = new OgmoLoader();
			loader.RegisterGridType("Collision", 16, 16);
			
			loader.RegisterClassAlias<JumpPad>("jumpPad");
			loader.RegisterClassAlias<Crumble>("crumble");
			loader.RegisterClassAlias<Razor>("deadlyAnchor");
			loader.RegisterClassAlias<SpawnPoint>("spawnPoint");
			loader.RegisterClassAlias<UpgradeSpawn>("Upgrade");
		}
		
		public Chunk(float posX, float posY) : base(posX, posY)
		{
			var world = new World();
			SpawnPoints = new List<Entity>();
			
			level = FP.Choose(levels);
			
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
				
				if (e.Mask is Grid)
				{
					var grid = e.Mask as Grid;
					
					var map = new Tilemap(Library.GetTexture("assets/tiles/Tileset.png"), FP.Width, FP.Height, 16, 16);
					AutoTileSet.CreateFromGrid(map, grid);
					
					e.AddComponent(map);
					e.Visible = true;
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
			
			Tweener.Tween(FP.Camera, new { Y = (int) (Y + FP.HalfHeight) }, 1)
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
