using Godot;
using System;
using System.Buffers.Binary;
using System.Threading;

public partial class Shotgun : Weapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		SHOTGUN CLASS

		Purpose: You know it

		What was Learned:
			How to set a random angle of spread
			The audio can get REAAALLY loud of all of the bullets have loud hitsounds
				I don't really know how to deal with this yet, maybe that's an use for having
				a specific AudioStreamPlayer, but at the same time, hearing 3 crit sounds
				overlayed but offset by a bit are very satisfying, but too much hurts
			I could def make this feel a lot better

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public float BulletCount = 10;

	[Export]
	public float SpreadAngle = 45;

	public override bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			TempAudio.PlayRandomPitch("reserve_shooter_01.wav", 0.2f, this);
			ShotCooldownTimer.Start();
			CanShoot = false;
			RandomNumberGenerator rng = new RandomNumberGenerator();
			for (int i = 0; i < BulletCount; i++)
			{
				var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2);
				//var angle = i;
				//if (i == 0) angle = 0;
				var proj = Projectiles[0].Instantiate<PlayerProjectile>();
				proj.Damage = this.Damage * 3 / BulletCount;
				proj.BaseSpeed = rng.RandfRange(280, 320);
				proj.CollisionLayer = 2;
				proj.Rotation = angle / 90;
				proj.Angle.X = angle / 90;
				proj.Angle.Y *= Mathf.Cos(angle * Mathf.Pi / 180);
				proj.Position = Position;

				ProjectileContainer.AddChild(proj);
			}
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
				TempAudio.PlayRandomPitch("reserve_shooter_01.wav", 0.2f, this);
				TempAudio.Play("crit_spawn.wav", this);
				ShotCooldownTimer.Start();
				CanShoot = false;
				RandomNumberGenerator rng = new RandomNumberGenerator();
				for (int i = 0; i < BulletCount; i++)
				{
					var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2);
					//var angle = i;
					//if (i == 0) angle = 0;
					var proj = Projectiles[0].Instantiate<PlayerProjectile>();
					proj.Damage = this.Damage * 3 * 3 / BulletCount ;
					proj.BaseSpeed = rng.RandfRange(280, 320);
					proj.CollisionLayer = 2;
					proj.Rotation = angle / 90;
					proj.Angle.X = angle / 90;
					proj.Angle.Y *= Mathf.Cos(angle * Mathf.Pi / 180);
					proj.Position = Position;
					proj.Hitsound = "crit_hit.wav";
					proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());

					ProjectileContainer.AddChild(proj);
				}
			}
			return false;
		}
		else return Shoot(Position);
	}

	public override void LoadProjectiles()
	{
		Projectiles.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Bullet.tscn"));
	}
}
