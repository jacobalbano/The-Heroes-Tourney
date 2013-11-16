/*
 * Created by SharpDevelop.
 * User: Quasar
 * Date: 11/16/2013
 * Time: 12:05 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;

namespace SNHU.GameObject.Platforms
{
	public class TeleporterOUT : Platform
	{
		private int ID;
			
		public int teleporterID
		{
			get
			{
				return ID;
			}
		}
		
		public TeleporterOUT()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint width = uint.Parse(node.Attributes["width"].Value);
			uint height = uint.Parse(node.Attributes["height"].Value);
			uint ID = uint.Parse(node.Attributes["ID"].Value);
			
			Graphic = myImage = Image.CreateRect(width, height, FP.Color(0x00FF00));
			SetHitboxTo(Graphic);
		} 
	}
}
