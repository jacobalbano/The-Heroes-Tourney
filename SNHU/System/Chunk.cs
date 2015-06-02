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
using SNHU.Config;
using SNHU.GameObject.Platforms;
using SNHU.Systems;

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
			levels = Library.GetFilenames("Levels/", "*")
				.ToList();
			
			for (int i = 0; i < levels.Length; i++)
			{
				var level = levels[i];
				level = level.Substring(level.LastIndexOf("/"));
				levels[i] = level;
			}
			
			l.Remove("/bawks.oel");
			l.Remove("/Real_3.oel");
			l.Remove("/bigYucky.oel");
			levels = l.ToArray();
			
//			levels = new string[] { "Real_3.oel" };
			
			loader = new OgmoLoader();
			loader.RegisterGridType("Collision", "Collision", 16, 16);
			
			loader.RegisterClassAlias<JumpPad>("jumpPad");
			loader.RegisterClassAlias<Crumble>("crumble");
			loader.RegisterClassAlias<Razor>("deadlyAnchor");
			loader.RegisterClassAlias<SpawnPoint>("spawnPoint");
			loader.RegisterClassAlias<UpgradeSpawn>("Upgrade");
		}
		
		public Chunk(float x, float y) : base(x, y)
		{
			var world = new World();
			SpawnPoints = new List<Entity>();
			
			level = FP.Choose.From(levels);
			ents = loader.BuildLevelAsArray(Library.GetXml("Levels/" + level));
			
			int spawns = 0;
			foreach (var e in ents)
			{
				if (!(e is Player))
				{
					e.X += X;
					e.Y += Y;
				}
				
				if (e is SpawnPoint)
				{
					++spawns;
					SpawnPoints.Add(e);
				}
				
				if (e.GetComponent<Grid>() != null)
				{
					var grid = e.GetComponent<Grid>();
					
					var map = new Tilemap(Library.GetTexture("tiles/Tileset.png"), FP.Width, FP.Height, 16, 16);
					AutoTileSet.CreateFromGrid(map, grid);
					
					e.AddComponent(map);
					e.Visible = true;
					e.Layer = ObjectLayers.Platforms;
				}
				
			}
			
			if (spawns != 4)
				throw new Exception("too few spawn points in" + level + ";" + spawns + "found, 4 required.");
		}
		
		
		public override void Added()
		{
			base.Added();
			
			World.AddList(ents);
			
			Tweener.Tween(FP.Camera, new { Y = Y + FP.HalfHeight }, 1)
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
			World.Add(new CameraManager(World.Camera.X, World.Camera.Y));
			
			World.BroadcastMessage(PlayerManager.Message.SpawnPlayers, SpawnPoints);
		}
	}
}
