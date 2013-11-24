/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 1:38 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of UpgradeSpawn.
	/// </summary>
	public class UpgradeSpawn : Entity
	{
		public UpgradeSpawn()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			
			if (FP.Rand(10) == 0)
			{
				switch (FP.Rand(8))
				{
					case 0:
						
						break;
					case 1:
						break;
					case 2:
						break;
					case 3:
						break;
					case 4:
						break;
					case 5:
						break;
					case 6:
						break;
					case 7:
						break;
					default:
						break;
				}
			}
		}
	}
}
