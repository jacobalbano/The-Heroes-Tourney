
using System;
using System.Collections.Generic;
using System.Linq;
using Indigo;
using Indigo.Audio;

namespace SNHU.Systems
{	
	public class SoundVariant
	{
		private Sound[] Sounds;
		private Sound CurrentSound;
		
		public SoundVariant(params Sound[] sounds)
		{
			Sounds = sounds;
		}
		
		public SoundVariant(IEnumerable<Sound> sounds)
		{
			Sounds = sounds.ToArray();
		}
		
		public SoundVariant(IEnumerable<string> files)
		{
			Sounds = files
				.Select(file => new Sound(Library.GetSoundBuffer(file)))
				.ToArray();
		}
		
		public void Play()
		{
			CurrentSound = FP.Choose.From(Sounds);
			CurrentSound.Play();
		}
		
		public void Pause()
		{
			if (CurrentSound != null)
				CurrentSound.Pause();
		}
		
		public void Stop()
		{
			if (CurrentSound != null)
				CurrentSound.Stop();
			
			CurrentSound = null;
		}
	}
}
