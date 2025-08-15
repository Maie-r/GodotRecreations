using Godot;
using System;
using System.IO;

public partial class Loose : Weapon, IChargeWeapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		LOOSE CLASS

		-Purpose: Represent how your ass is like reading this-
		Purpose: The loose cannon from tf2

		What was Learned:
			I could prob have gone without the IChargeWeapon interface, but I think that
				would cause a lot of headaches, but it resulted in having to add more logic
				to the player class itself
			Double donks are really fun

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public float MaxChargeTime = 2;
	[Export]
	public float SpreadAngle = 10;

	public bool isCharging = false;
	public Timer ChargeTimer;
	bool CritNext = false;

	public AudioStreamPlayer2D ChargeSound;

	public override bool Shoot(Vector2 Position)
	{
		//GD.Print(ChargeTimer.TimeLeft);
		if (CanShoot)
		{
			ChargeSound.Play();
			CanShoot = false;
			isCharging = true;
			CritNext = false;
			ChargeTimer.Start();
		}
		return false;
	}

	public override bool Shoot(bool RandomCrit, Vector2 Position)
	{
		//GD.Print(ChargeTimer.TimeLeft);
		if (RandomCrit)
		{
			if (CanShoot)
			{
				ChargeSound.Play();
				CanShoot = false;
				isCharging = true;
				CritNext = true;
				ChargeTimer.Start();
			}
			return false;
		}
		else return Shoot(Position);
	}

	public bool ChargeStop()
	{
		if (isCharging)
		{
			ChargeSound.Stop();
			isCharging = false;
			ShotCooldownTimer.Start();
			
			RandomNumberGenerator rng = new RandomNumberGenerator();
			TempAudio.PlayRandomPitch("loose_cannon_shoot.wav", 0.2f, this);
			ShotCooldownTimer.Start();
			CanShoot = false;
			var proj = Projectiles[0].Instantiate<CannonBall>();
			proj.BaseSpeed *= 1.2f;
			proj.Damage = this.Damage * proj.Multiplier;
			proj.CollisionLayer = 2;
			proj.Position = SpawnPos.GlobalPosition;
			proj.Hitsound = "loose_cannon_ball_impact.wav";
			var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
			proj.Rotation = angle;
			proj.Angle.X += angle;
			proj.ExplodeAfter = (float)ChargeTimer.TimeLeft;
			ChargeTimer.Stop();

			ProjectileContainer.AddChild(proj);

			ResetTimer();
			return true;
		}
		return false;
	}

	public bool ChargeStop(bool Crit)
	{
		if (Crit)
		{
			if (isCharging)
			{
				ChargeSound.Stop();
				isCharging = false;
				ShotCooldownTimer.Start();
				CanShoot = false;

				RandomNumberGenerator rng = new RandomNumberGenerator();
				TempAudio.PlayRandomPitch("loose_cannon_shoot.wav", 0.2f, this);
				TempAudio.Play("crit_spawn.wav", this);
				var proj = Projectiles[0].Instantiate<CannonBall>();
				proj.BaseSpeed *= 1.2f;
				proj.Damage = this.Damage * proj.Multiplier * 3;
				proj.CollisionLayer = 2;
				proj.Position = SpawnPos.GlobalPosition;
				proj.Hitsound = "crit_hit.wav";
				proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());
				var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
				proj.Rotation = angle;
				proj.Angle.X += angle;
				proj.ExplodeAfter = (float)ChargeTimer.TimeLeft;
				ChargeTimer.Stop();

				ProjectileContainer.AddChild(proj);

				ResetTimer();
				return true;
			}
		}
		return ChargeStop();
	}

	public override void LoadProjectiles()
	{
		Projectiles.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/CannonBall.tscn"));
	}

	public override void LoadAdditional()
	{
		ChargeTimer = new Timer();
		ResetTimer();
		ChargeTimer.Timeout += ExplodeInFace;
		AddChild(ChargeTimer);
		ChargeSound = new AudioStreamPlayer2D();
		ChargeSound.Stream = ResourceLoader.Load<AudioStream>("res://BulletHell/assets/sfx/stickybomblauncher_charge_up.wav");
		AddChild(ChargeSound);
	}

	void ResetTimer()
	{
		ChargeTimer.Stop();
		ChargeTimer.WaitTime = MaxChargeTime;
	}

	void ExplodeInFace()
	{
		GD.Print("EXPLODE IN MA FACE");
		if (isCharging)
		{
			ChargeSound.Stop();
			isCharging = false;
			ShotCooldownTimer.Start();
			CanShoot = false;
			ChargeTimer.Stop();

			TempAudio.PlayRandomPitch("grenade_launcher_shoot.wav", 0.2f, this);
			var proj = Projectiles[0].Instantiate<CannonBall>();
			proj.Damage = this.Damage * proj.Multiplier;
			if (CritNext)
			{
				TempAudio.Play("crit_spawn.wav", this);
				proj.Hitsound = "crit_hit.wav";
				proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());
				proj.Damage *= 3;
			}
			proj.CollisionLayer = 2;
			proj.Position = this.GlobalPosition;
			Timer tt = TempTimer.Get(0.05f);
			tt.Timeout += proj.Explode;
			AddChild(tt);
			ProjectileContainer.AddChild(proj);
			GetTree().CallGroup("Player", "TakeDamage", 1); // Hacky way, needs to fix player detection in explosion
			ResetTimer();
		}
	}

	public bool IsCharging()
	{
		return isCharging;
	}
}

public interface IChargeWeapon
{
	public bool IsCharging();
	public bool ChargeStop(bool Crit);
	public bool ChargeStop();
}
