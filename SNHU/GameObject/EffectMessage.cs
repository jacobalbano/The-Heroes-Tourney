
using System;
using Indigo;

namespace SNHU.GameObject.Upgrades
{
	/// <summary>
	/// Description of Struct1.
	/// </summary>
	public struct EffectMessage
	{
		public enum Message { OnEffect }
		
		public delegate void Callback(Entity from, Entity to, float scalar);
		public Callback Apply { get; private set; }
		public Entity Sender { get; private set; }
		
		public EffectMessage(Entity sender, Callback callback) : this()
		{
			Apply = callback;
			Sender = sender;
		}
	}
}
