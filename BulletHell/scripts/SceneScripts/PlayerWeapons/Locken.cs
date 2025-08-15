using Godot;
using System;

public partial class Locken : Weapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		LOCKEN CLASS

		Purpose: Like the Lock'n Load from tf2

		What was Learned:
			Damn the lock'n load is actually kinda boring, so I made this a double tap weapon

	*/////////////////////////////////////////////////////////////////////////////////
	[Export]
	float SpreadAngle = 5;
	public override bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			ShotCooldownTimer.Start();
			CanShoot = false;
			Timer timer = new Timer();
			timer.WaitTime = 0.1f;
			timer.Autostart = true;
			timer.Timeout += () => { Sht(Position); timer.QueueFree(); };
			Sht(Position);
			AddChild(timer);
			timer.Start();
			return true;
		}
		return false;
	}

	public override bool Shoot(bool RandomCrit, Vector2 Position)
	{
		if (RandomCrit)
		{
			if (CanShoot)
			{
				ShotCooldownTimer.Start();
				CanShoot = false;
				Timer timer = new Timer();
				timer.WaitTime = 0.1f;
				timer.Autostart = true;
				timer.Timeout += () => { Sht(RandomCrit, Position); timer.QueueFree(); };
				Sht(RandomCrit, Position);
				AddChild(timer);
				return true;
			}
			return false;
		}
		else return Shoot(Position);
	}

	void Sht(Vector2 Position)
	{
		RandomNumberGenerator rng = new RandomNumberGenerator();
		TempAudio.PlayRandomPitch("grenade_launcher_shoot.wav", 0.3f, this);
		var proj = Projectiles[0].Instantiate<Pipe>();
		proj.ExplosionRadius = 80;
		proj.BaseSpeed *= 3;
		proj.Damage = this.Damage * proj.Multiplier * 1.2f;
		proj.CollisionLayer = 2;
		proj.Position = Position;
		var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
		proj.Rotation = angle;
		proj.Angle.X += angle;

		ProjectileContainer.AddChild(proj);
	}

	void Sht(bool Crit, Vector2 Position)
	{
		if (!Crit) Sht(Position);
		else
		{
			RandomNumberGenerator rng = new RandomNumberGenerator();
			TempAudio.PlayRandomPitch("grenade_launcher_shoot.wav", 0.2f, this);
			TempAudio.Play("crit_spawn.wav", this);
			var proj = Projectiles[0].Instantiate<Pipe>();
			proj.ExplosionRadius = 80;
			proj.BaseSpeed *= 3;
			proj.Damage = this.Damage * proj.Multiplier * 1.2f * 3;
			proj.CollisionLayer = 2;
			proj.Position = Position;
			proj.Hitsound = "crit_hit.wav";
			proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());
			var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
			proj.Rotation = angle;
			proj.Angle.X += angle;

			ProjectileContainer.AddChild(proj);
		}
	}

	public override void LoadProjectiles()
	{
		Projectiles.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Pipe.tscn"));
	}
}
