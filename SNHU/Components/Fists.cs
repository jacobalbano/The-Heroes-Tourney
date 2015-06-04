
using System;
using Indigo;
using Indigo.Core;
using SNHU.GameObject;

namespace SNHU.Components
{
	public class Fists : Component
	{
		private Fist[] fists;
		private int hand;
		private bool _guarding;
		private float _alpha, _forceMultiplier;
		
		public float ForceMultiplier
		{
			get { return _forceMultiplier; }
			set
			{
				for (int i = 0; i < fists.Length; i++)
					fists[i].ForceMultiplier = value;
				
				_forceMultiplier = value;
			}
		}
		
		public float Alpha
		{
			get { return _alpha; }
			set
			{
				for (int i = 0; i < fists.Length; i++)
					fists[i].Image.Alpha = value;
				
				_alpha = value;
			}
		}
		
		public bool Guarding
		{
			get { return _guarding; }
			set
			{
				for (int i = 0; i < fists.Length; i++)
					fists[i].SetGuarding(value);
				
				_guarding = value;
			}
		}
		
		public bool Punching
		{
			get
			{
				for (int i = 0; i < fists.Length; i++)
					if (fists[i].Punching) return true;
				
				return false;
			}
		}
		
		public Fists()
		{
			fists = new Fist[2];
			_forceMultiplier = Fist.DEFAULT_PUNCH_MULT;
			_alpha = 1;
			_guarding = false;
		}
		
		public override void Added()
		{
			base.Added();
			fists[0] = new Fist(true, Parent as Player);
			fists[1] = new Fist(false, Parent as Player);
		}
		
		public override void ParentAdded()
		{
			base.ParentAdded();
			Parent.World.AddList(fists);
		}
		
		public override void ParentRemoved()
		{
			base.ParentRemoved();
			Parent.World.RemoveList(fists);
		}
		
		public void FaceLeft()
		{
			for (int i = 0; i < fists.Length; i++)
				fists[i].FaceLeft();
		}
		
		public void FaceRight()
		{
			for (int i = 0; i < fists.Length; i++)
				fists[i].FaceRight();
		}
		
		public bool Punch(float dirX, float dirY)
		{
			var result = fists[hand].Punch(dirX, dirY);
			if (result)
			{
				hand++;
				hand %= fists.Length;
			}
			
			return result;
		}
	}
}
