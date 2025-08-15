using Godot;
using System;
using System.Collections.Generic;

public partial class Gun : Weapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		GUN CLASS

		Purpose: The simplest weapon, that fires a simple projectile

		What was Learned:
			Well, that this was viable at all

	*/////////////////////////////////////////////////////////////////////////////////

	public override bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			TempAudio.PlayRandomPitch("revolver_shoot.wav", 0.2f, -7, this);
			ShotCooldownTimer.Start();
			CanShoot = false;
			var proj = Projectiles[0].Instantiate<PlayerProjectile>();
			proj.CollisionLayer = 2;
			proj.Position = Position;
			proj.Damage = Damage;

			ProjectileContainer.AddChild(proj);
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
				TempAudio.PlayRandomPitch("revolver_shoot.wav", 0.2f, -7, this);
				TempAudio.Play("crit_spawn.wav", this);
				ShotCooldownTimer.Start();
				CanShoot = false;
				var proj = Projectiles[0].Instantiate<PlayerProjectile>();
				proj.CollisionLayer = 2;
				proj.Position = Position;
				proj.Damage = Damage * 3;
				proj.Hitsound = "crit_hit.wav";
				proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());

				ProjectileContainer.AddChild(proj);

				return true;
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
