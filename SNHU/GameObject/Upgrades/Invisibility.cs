/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/24/2013
 * Time: 1:52 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Invisibility.
	/// </summary>
	public class Invisibility : Upgrade
	{
		const float INVIS_ALPHA = 0.03f;
		
		public Invisibility()
		{
			//image = new Image(Library.GetTexture("assets/" +  + ".png"));
			image = Image.CreateCircle(3, FP.Color(0xFF0000));
		}
		
		public override void Use()
		{
			if (!Activated)
			{
				base.Use();
			
				var tweenPlayer = new VarTween(null, Tween.ONESHOT);
				tweenPlayer.Tween((Parent as Player).player, "Alpha", INVIS_ALPHA, 0.25f);
				Parent.AddTween(tweenPlayer, true);
				
				var tweenFist1 = new VarTween(null, Tween.ONESHOT);
				tweenFist1.Tween(((Parent as Player).left.Graphic as Image), "Alpha", INVIS_ALPHA, 0.25f);
				Parent.AddTween(tweenFist1, true);
				
				var tweenFist2 = new VarTween(null, Tween.ONESHOT);
				tweenFist2.Tween(((Parent as Player).right.Graphic as Image), "Alpha", INVIS_ALPHA, 0.25f);
				Parent.AddTween(tweenFist2, true);
			}
		}
		
		public override void OnLifetimeComplete()
		{
			base.OnLifetimeComplete();
			
			var tweenPlayer = new VarTween(null, Tween.ONESHOT);
			tweenPlayer.Tween((Parent as Player).player, "Alpha", 1.0f, 0.25f);
			Parent.AddTween(tweenPlayer, true);
			
			var tweenFist1 = new VarTween(null, Tween.ONESHOT);
			tweenFist1.Tween(((Parent as Player).left.Graphic as Image), "Alpha", 1.0f, 0.25f);
			Parent.AddTween(tweenFist1, true);
			
			var tweenFist2 = new VarTween(null, Tween.ONESHOT);
			tweenFist2.Tween(((Parent as Player).right.Graphic as Image), "Alpha", 1.0f, 0.25f);
			Parent.AddTween(tweenFist2, true);
			
			(Parent as Player).SetUpgrade(null);
		}
	}
}
