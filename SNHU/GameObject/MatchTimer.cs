/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 12:45 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Punk;
using Punk.Graphics;
using Punk.Tweens.Misc;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of MatchTimer.
	/// </summary>
	public class MatchTimer : Entity
	{
		private float duration;
		public Alarm Timer;
		
		public MatchTimer(float durationSeconds = 240.0f)
		{
			duration = durationSeconds;
			Timer = new Alarm(duration, OnComplete, Tween.ONESHOT);
			
			Graphic = new Text((Timer.Remaining).ToString("0.00"));
			(Graphic as Text).Size = 36;
			Graphic.ScrollX = Graphic.ScrollY = 0;
			
			X = (FP.Width / 2) - (Graphic as Text).Width;
			Y = 20;
		}
		
		public override void Added()
		{
			base.Added();
			
			Layer = -1000;
			
			World.AddTween(Timer, false);
		}
		
		public override void Update()
		{
			base.Update();
			
			(Graphic as Text).String = (Timer.Remaining).ToString("0.00");
			X = (FP.Width / 2) - (Graphic as Text).Width;
		}
		
		private void OnComplete()
		{
			
		}
	}
}
