using System;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	/// <summary>
	/// Description of FloorButton.
	/// </summary>
	[OgmoConstructor("Group", "width")]
	public class FloorButton : Trigger
	{
		public float Duration { get; set; }
		public float Cooldown { get; set; }
		public bool OnCooldown { get; private set; }
		private bool collidingWithPlayer;
		
		public FloorButton(string group, int width) : base(group)
		{
			var image = Image.CreateRect(width, 16, FP.Color(0x00ffff));
			
			Width = width;
			Height = 16;
			
			AddComponent(image);
		}
		
		public override void Added()
		{
			base.Added();
			Y += 16;
		}
		
		public override void Update()
		{
			base.Update();
			
			var player = Collide(Player.Collision, X, Y);
			
			if (player != null)
			{
				if (!collidingWithPlayer)
				{
					TriggerGroupOn();
					collidingWithPlayer = true;
				}
			}
			else
			{
				if (collidingWithPlayer)
				{
					TriggerGroupOff();
					collidingWithPlayer = false;
				}
			}
		}
	}
}
