/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 11/16/2013
 * Time: 12:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
			Audio.Add("hit2", new Sound(Library.GetBuffer("assets/audio/hit2.wav")));
			Audio.Add("land1", new Sound(Library.GetBuffer("assets/audio/land1.wav")));
			Audio.Add("death1", new Sound(Library.GetBuffer("assets/audio/death1.wav")));
			Audio.Add("crumble", new Sound(Library.GetBuffer("assets/audio/crumble.wav")));
			Audio.Add("teleport", new Sound(Library.GetBuffer("assets/audio/teleport.wav")));
			Audio.Add("jumpPad", new Sound(Library.GetBuffer("assets/audio/jumpPad.wav")));
//			Audio.Add("music", new Sound(Library.GetBuffer("assets/audio/music.ogg")));
			
			music = Library.GetMusic("assets/audio/music.ogg");
		}
	}
}
