using System;
using Indigo;
using Indigo.Graphics;
using SNHU.GameObject.Platforms;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	public class Trigger : Entity
	{
		public string Group { get; set; }
		
		public enum Message
		{
			On,
			Off
		}
		
		public Trigger(string group = "default")
		{
			Group = group;
		}
		
		protected void TriggerGroupOn()
		{
			World.BroadcastMessage(Message.On, Group);
		}
		
		protected void TriggerGroupOff()
		{
			World.BroadcastMessage(Message.Off, Group);
		}
	}
}
