using System;
using System.Collections.Generic;
using Punk;
using SFML.Audio;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of Mixer.
	/// </summary>
	public static class Mixer
	{
		public static readonly Sound
			Jump1,
			Jump2,
			Jump3,
			Hit1,
			Land1,
			Death1,
			JumpPad,
			BulletBounce,
			ReboundUp,
			ReboundDown,
			ShieldUp,
			ShieldDown,
			Explode,
			TimeTick,
			SawHit,
			Fus,
			Swing1,
			Swing2;
		
		public static Punk.Music music;
		
		static Mixer()
		{
			Jump1 =  new Sound(Library.GetBuffer("assets/audio/jump1.wav"));
			Jump2 = new Sound(Library.GetBuffer("assets/audio/jump2.wav"));
			Jump3 = new Sound(Library.GetBuffer("assets/audio/jump3.wav"));
			Hit1 = new Sound(Library.GetBuffer("assets/audio/hit1.wav"));
			Land1 =  new Sound(Library.GetBuffer("assets/audio/land1.wav"));
			Death1 = new Sound(Library.GetBuffer("assets/audio/death1.wav"));
			JumpPad = new Sound(Library.GetBuffer("assets/audio/jumpPad.wav"));
			BulletBounce = new Sound(Library.GetBuffer("assets/audio/bulletBounce.wav"));
			ReboundUp = new Sound(Library.GetBuffer("assets/audio/reboundUp.wav"));
			ReboundDown = new Sound(Library.GetBuffer("assets/audio/reboundDown.wav"));
			ShieldUp = new Sound(Library.GetBuffer("assets/audio/shieldUp.wav"));
			ShieldDown = new Sound(Library.GetBuffer("assets/audio/shieldDown.wav"));
			Explode = new Sound(Library.GetBuffer("assets/audio/explode.wav"));
			TimeTick = new Sound(Library.GetBuffer("assets/audio/timeTick.wav"));
			SawHit = new Sound(Library.GetBuffer("assets/audio/sawHit.wav"));
			Fus = new Sound(Library.GetBuffer("assets/audio/fus.wav"));
			Swing1 = new Sound(Library.GetBuffer("assets/audio/swing1.wav"));
			Swing2 = new Sound(Library.GetBuffer("assets/audio/swing2.wav"));
			
			music = Library.GetMusic("assets/audio/music" + FP.Choose(2, 1) + ".ogg");
			
//			music.Volume = 0;
		}
	}
}
