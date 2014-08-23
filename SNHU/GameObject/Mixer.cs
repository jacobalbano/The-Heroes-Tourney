using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Audio;

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
		
		public static Indigo.Audio.Sound music;
		
		static Mixer()
		{
			Jump1 =  new Sound(Library.GetSoundBuffer("assets/audio/jump1.wav"));
			Jump2 = new Sound(Library.GetSoundBuffer("assets/audio/jump2.wav"));
			Jump3 = new Sound(Library.GetSoundBuffer("assets/audio/jump3.wav"));
			Hit1 = new Sound(Library.GetSoundBuffer("assets/audio/hit1.wav"));
			Land1 =  new Sound(Library.GetSoundBuffer("assets/audio/land1.wav"));
			Death1 = new Sound(Library.GetSoundBuffer("assets/audio/death1.wav"));
			JumpPad = new Sound(Library.GetSoundBuffer("assets/audio/jumpPad.wav"));
			BulletBounce = new Sound(Library.GetSoundBuffer("assets/audio/bulletBounce.wav"));
			ReboundUp = new Sound(Library.GetSoundBuffer("assets/audio/reboundUp.wav"));
			ReboundDown = new Sound(Library.GetSoundBuffer("assets/audio/reboundDown.wav"));
			ShieldUp = new Sound(Library.GetSoundBuffer("assets/audio/shieldUp.wav"));
			ShieldDown = new Sound(Library.GetSoundBuffer("assets/audio/shieldDown.wav"));
			Explode = new Sound(Library.GetSoundBuffer("assets/audio/explode.wav"));
			TimeTick = new Sound(Library.GetSoundBuffer("assets/audio/timeTick.wav"));
			SawHit = new Sound(Library.GetSoundBuffer("assets/audio/sawHit.wav"));
			Fus = new Sound(Library.GetSoundBuffer("assets/audio/fus.wav"));
			Swing1 = new Sound(Library.GetSoundBuffer("assets/audio/swing1.wav"));
			Swing2 = new Sound(Library.GetSoundBuffer("assets/audio/swing2.wav"));
			
			music = new Sound(Library.GetSoundStream("assets/audio/music2.ogg")); // lol wot pls
			
//			music.Volume = 0;
		}
	}
}
