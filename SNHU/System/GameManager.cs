using System;
using System.Collections.Generic;
using System.Linq;
using Glide;
using Indigo;
using Indigo.Audio;
using Indigo.Utils;
using Indigo.Graphics;
using SNHU.Config;
using SNHU.MenuObject;
using SNHU.Systems;

namespace SNHU.GameObject
{
	/// <summary>
	/// Description of GameManager.
	/// </summary>
	public class GameManager : Entity
	{
		private bool paused;
		private Sound music;
		
		public GameManager()
		{
			music = Mixer.Music;
		}
		
		public void StartGame()
		{
			music.Looping = true;
			music.Play();
		}
		
		public void TogglePauseGame(bool affectEnts)
		{
			List<Entity> entList = new List<Entity>();
			World.GetAll(entList);
			
			paused = !paused;
			foreach (Entity e in entList)
				e.Active = paused;
			
			if (paused)
			{
				music.Pause();
			}
			else
			{
				music.Play();
			}
		}
	}
}
