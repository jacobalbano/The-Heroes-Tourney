/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/23/2013
 * Time: 9:16 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;

namespace SNHU
{
	/// <summary>
	/// Description of OffscreenCursor.
	/// </summary>
	public class OffscreenCursor : Entity
	{
		Entity target;
		public OffscreenCursor(Entity target) : base(0,0, Image.CreateCircle(30, FP.Color(0xFFFFFF)))
		{
			this.target = target;
			(Graphic as Image).CenterOrigin();
		}
		
		public override void Added()
		{
			base.Added();
			
			Layer = -1000;
		}
		
		public override void Removed()
		{
			base.Removed();
		}
		
		public override void Update()
		{
			base.Update();
			
			X = target.X - target.HalfWidth;
			Y = target.Y - target.HalfHeight;
			FP.ClampInRect(ref X, ref Y, FP.Camera.X - FP.HalfWidth, FP.Camera.Y - FP.HalfHeight, FP.Width, FP.Height, 16);
			
			FP.Log(target.X, target.Y);
		}
	}
}
