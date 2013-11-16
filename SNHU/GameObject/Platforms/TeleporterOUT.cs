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
	public class TeleporterOUT : Teleporter
	{

		
		public TeleporterOUT() : base()
		{
		}
		
		public override void Load(System.Xml.XmlNode node)
		{
			base.Load(node);
			uint ID = uint.Parse(node.Attributes["ID"].Value);
			
			Graphic = myImage = Image.CreateRect(32, 32, FP.Color(0xFFFF00));
			SetHitboxTo(Graphic);
		} 
	}
}
