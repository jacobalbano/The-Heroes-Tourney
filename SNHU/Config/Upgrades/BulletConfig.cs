
using System;
using Indigo.Content.Data;

namespace SNHU.Config.Upgrades
{
	public class BulletConfig : IniConfig
	{
		public enum BulletMode { Cone, Radial, Random }
		
		public int Count = 3;
		public float Lifetime = 5;
		
		[EnumComment]
		public BulletMode Mode;
	}
}
