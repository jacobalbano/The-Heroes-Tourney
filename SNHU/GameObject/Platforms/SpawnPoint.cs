using System;
using Punk;

namespace SNHU.GameObject.Platforms
{
	/// <summary>
	/// Description of SpawnPoint.
	/// </summary>
	public class SpawnPoint : Entity
	{
		public const string Collision = "spawnPoint";
		
		public SpawnPoint() : base()
		{
			Type = Collision;
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
			Type = Collision;
		}
		
		public override void Added()
		{
			base.Added();
		}
	}
}
