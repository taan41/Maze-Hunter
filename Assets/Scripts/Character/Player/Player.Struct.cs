using System;

public partial class Player
{
	public enum ActionState : int
	{
		Idle = 0,
		Walk = 1,
		Sprint = 2,
		Jump = 3,
		Roll = 4,
		Attack = 5,
	}
	
	public enum WeaponType : int
	{
		None = 0,
		Sword = 1,
		Gun = 2,
	}

	[Serializable]
	public class SwordSettings
	{
		public float damage = 10f;
		public float energyOnHit = 3f;
		public float empoweredMultiplier = 2f;
		public float empoweredDuration = 10f;
		public int comboMax = 4;
		public float comboResetTime = 1f;
		public float staggerSpeed = 5f;
		public float queueAttackInputDelay = 0.8f;
		public float hitIgnoreDuration = 0.3f;
	}

	[Serializable]
	public class GunSettings
	{
		[Serializable]
		public class SpreadSettings
		{
			public float _base = 15f;
			public float walkDelta = 6f;
			public float jumpDelta = 15f;
			public float shootDelta = 10f;
			public float shootDecrease = 60f;
			public float shootMax = 30f;
		}

		public SpreadSettings spread = new();
		public float damage = 5f;
		public float shootCooldown = 0.15f;
		public float staggerSpeed = 3f;
		public float autoReloadWaitTime = 2f;
	}
}