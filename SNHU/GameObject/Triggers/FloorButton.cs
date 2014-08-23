/*
 * Created by SharpDevelop.
 * User: Chris
 * Date: 8/23/2014
 * Time: 3:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Indigo;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	/// <summary>
	/// Description of FloorButton.
	/// </summary>
	[OgmoConstructor("group", "duration", "cooldown")]
	public class FloorButton : Trigger
	{
		public float Duration { get; set; }
		public float Cooldown { get; set; }
		public bool OnCooldown { get; private set; }
		
		public FloorButton(string group, float duration = 0.0f, float cooldown = 1.0f) : base(group)
		{
			Duration = duration;
			Cooldown = cooldown;
			OnCooldown = false;
			
			
			
			AddResponse(Player.Message.OnLand, OnPlayerLand);
		}
		
		
		public void OnPlayerLand(params object[] args)
        {
			if (!OnCooldown)
			{
				var player = (Player)args[0];
				
				if (player.CollideWith(this, X, Y) != null)
				{
					FP.Log("I'm being stepped on :(   Group: " + Group);
					
					World.BroadcastMessage(Message.On, Group);
					
					if (Duration > 0.0f)
					{
						Tweener.Timer(Duration).OnComplete(OnDurationComplete);
					}
				}
			}
        }
		
		public void OnDurationComplete()
        {
			FP.Log("I'm not being stepped on anymore :(   Group: " + Group);
			
			if (Cooldown > 0.0f)
			{
				OnCooldown = true;
				Tweener.Timer(Cooldown).OnComplete(OnCooldownComplete);
			}
        }
		
		public void OnCooldownComplete()
        {
			FP.Log("I'm off cooldown   Group: " + Group);
			
			OnCooldown = false;
        }
	}
}
