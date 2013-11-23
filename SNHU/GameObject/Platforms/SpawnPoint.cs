/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 10:02 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
