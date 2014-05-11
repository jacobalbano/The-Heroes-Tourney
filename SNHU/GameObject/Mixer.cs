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
		public static Dictionary<string, Sound> Audio;
		
		public static Punk.Music music;
		
		static Mixer()
		{
			Audio = new Dictionary<string, Sound>();
			Audio.Add("jump1", new Sound(Library.GetBuffer("assets/audio/jump1.wav")));
			Audio.Add("jump2", new Sound(Library.GetBuffer("assets/audio/jump2.wav")));
			Audio.Add("jump3", new Sound(Library.GetBuffer("assets/audio/jump3.wav")));
			Audio.Add("hit1", new Sound(Library.GetBuffer("assets/audio/hit1.wav")));
			Audio.Add("land1", new Sound(Library.GetBuffer("assets/audio/land1.wav")));
			Audio.Add("death1", new Sound(Library.GetBuffer("assets/audio/death1.wav")));
			Audio.Add("jumpPad", new Sound(Library.GetBuffer("assets/audio/jumpPad.wav")));
			Audio.Add("bulletBounce", new Sound(Library.GetBuffer("assets/audio/bulletBounce.wav")));
			Audio.Add("reboundUp", new Sound(Library.GetBuffer("assets/audio/reboundUp.wav")));
			Audio.Add("reboundDown", new Sound(Library.GetBuffer("assets/audio/reboundDown.wav")));
			Audio.Add("shieldUp", new Sound(Library.GetBuffer("assets/audio/shieldUp.wav")));
			Audio.Add("shieldDown", new Sound(Library.GetBuffer("assets/audio/shieldDown.wav")));
			Audio.Add("explode", new Sound(Library.GetBuffer("assets/audio/explode.wav")));
			Audio.Add("timeTick", new Sound(Library.GetBuffer("assets/audio/timeTick.wav")));
			Audio.Add("sawHit", new Sound(Library.GetBuffer("assets/audio/sawHit.wav")));
			Audio.Add("fus", new Sound(Library.GetBuffer("assets/audio/fus.wav")));
			Audio.Add("swing1", new Sound(Library.GetBuffer("assets/audio/swing1.wav")));
			Audio.Add("swing2", new Sound(Library.GetBuffer("assets/audio/swing2.wav")));
			
			music = Library.GetMusic("assets/audio/music" + FP.Choose(2, 1) + ".ogg");
			
//			music.Volume = 0;
		}
	}
}
