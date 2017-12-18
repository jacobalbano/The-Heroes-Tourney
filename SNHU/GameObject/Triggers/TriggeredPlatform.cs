﻿using System;
using Glide;
using Indigo;
using Indigo.Colliders;
using Indigo.Graphics;
using Indigo.Loaders;

namespace SNHU.GameObject.Triggers
{
	public class TriggeredPlatform : Triggerable
	{
		private Image image;
		private Hitbox hitbox;
		private const float Duration = 0.25f;
		
		[OgmoConstructor("Group", "width", "height")]
		public TriggeredPlatform(string group, int width, int height)
		{
			AddComponent(image = Image.CreateRect(width, height, new Color(0x400080)));
			hitbox = new Hitbox(width, height);
		}
		
		public override void Added()
		{
			base.Added();

			hitbox.CenterOrigin();
			image.CenterOrigin();
			X += hitbox.Width / 2;
			Y += hitbox.Height / 2;
		}
		
		protected override void TriggerOn()
		{
			base.TriggerOff();
			
			Tween tween = null;
			if (hitbox.Width < hitbox.Height) tween = Tweener.Tween(image, new { ScaleX = 0 }, Duration);
			else tween = Tweener.Tween(image, new { ScaleY = 0 }, 0.5f);
			
			tween.Ease(Ease.BackIn);
			Collidable = false;
		}
		
		protected override void TriggerOff()
		{
			base.TriggerOn();
			
			Tween tween = null;
			if (hitbox.Width < hitbox.Height) tween = Tweener.Tween(image, new { ScaleX = 1 }, Duration);
			else tween = Tweener.Tween(image, new { ScaleY = 1 }, 0.5f);
			
			tween.Ease(Ease.BackOut);
			Collidable = true;
		}
	}
}