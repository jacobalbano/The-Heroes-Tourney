
using System;
using Glide;
using Indigo;
using Indigo.Graphics;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	[OgmoConstructor("Duration", "Group")]
	public class TimerTrigger : Trigger
	{
		private float Duration;
		private Message lastMessage;
		private Image face, hand;
		
		public TimerTrigger(float duration, string group) : base(group)
		{
			Duration = duration;
			lastMessage = Message.On;
			
			face = Image.CreateCircle(30, FP.Color(0xffffff));
			face.CenterOO();
			
			hand = Image.CreateRect(5, 30, FP.Color(0));
			hand.OriginX = hand.Width / 2;
			hand.OriginY = hand.Height - hand.OriginX;
			
			AddComponent(face);
			AddComponent(hand);
			
			Tweener.Tween(hand, new { Angle = -360}, duration)
				.OnComplete(OnTick)
				.Repeat();
		}
		
		private void OnTick()
		{
			lastMessage = lastMessage == Message.Off ? Message.On : Message.Off;
			World.BroadcastMessage(lastMessage);
			
			face.Scale = 1.5f;
			Tweener.Tween(face, new { Scale = 1 }, Duration / 2)
				.Ease(Ease.SineOut);
		}
	}
}
