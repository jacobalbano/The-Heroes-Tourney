/*
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
		public Chunk(float posX, float posY):base(posX,posY)
		{
			
		}
		
		public override void Added()
		{
			base.Added();
			
			var world = new World();
			world.RegisterClass<Platform>("platform");
			
			var ents = world.BuildWorldAsArray("assets/Levels/Test2.oel");
			
			foreach (var e in ents)
			{
				e.X += X;
				e.Y += Y;
				e.Layer = -100;
			}
			
			World.AddList(ents);
		}
	}
}
