using System;
using System.Collections.Generic;
using Indigo;
using Indigo.Audio;

namespace SNHU.Systems
{
	/// <summary>
	/// Description of Mixer.
	/// </summary>
	public class Mixer
	{
		public static readonly Sound
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
			Fus;
			
		public static readonly SoundVariant
			Jump,
			Swing,
			DoubleJump;
		
		public static Sound Music;
		
		static Mixer()
		{
			Jump = new SoundVariant(Library.GetFilenames("audio/", "jump*.wav"));
			Swing = new SoundVariant(Library.GetFilenames("audio/", "swing*.wav"));
			DoubleJump = new SoundVariant(Library.GetFilenames("audio/", "doublejump*.wav"));
			
			Hit1 = new Sound(Library.GetSoundBuffer("audio/hit1.wav"));
			Land1 =  new Sound(Library.GetSoundBuffer("audio/land1.wav"));
			Death1 = new Sound(Library.GetSoundBuffer("audio/death1.wav"));
			JumpPad = new Sound(Library.GetSoundBuffer("audio/jumpPad.wav"));
			BulletBounce = new Sound(Library.GetSoundBuffer("audio/bulletBounce.wav"));
			ReboundUp = new Sound(Library.GetSoundBuffer("audio/reboundUp.wav"));
			ReboundDown = new Sound(Library.GetSoundBuffer("audio/reboundDown.wav"));
			ShieldUp = new Sound(Library.GetSoundBuffer("audio/shieldUp.wav"));
			ShieldDown = new Sound(Library.GetSoundBuffer("audio/shieldDown.wav"));
			Explode = new Sound(Library.GetSoundBuffer("audio/explode.wav"));
			TimeTick = new Sound(Library.GetSoundBuffer("audio/timeTick.wav"));
			SawHit = new Sound(Library.GetSoundBuffer("audio/sawHit.wav"));
			Fus = new Sound(Library.GetSoundBuffer("audio/fus.wav"));
			
			Music = new Sound(Library.GetSoundStream("audio/music2.ogg")); // lol wot pls
//			Music.Volume = 0;
		}
		
		public static void Preload() { }
	}
}
