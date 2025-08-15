using Godot;
using System;

public partial class Stock : Weapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		STOCK CLASS

		Purpose: It's the stock grenade launcher for the Demoman in tf2

		What was Learned:
			You CAN make a projectile spawn a projectile, but it can be very scary if they are
				Parent and children, disposing of one will dispose the other (and their sounds)
				So its best to have a reference to your projectiles container and have your projectile
				spawn another in there instead of as it's child

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	float SpreadAngle = 10;
	public override bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			RandomNumberGenerator rng = new RandomNumberGenerator();
			TempAudio.PlayRandomPitch("grenade_launcher_shoot.wav", 0.2f, this);
			ShotCooldownTimer.Start();
			CanShoot = false;
			var proj = Projectiles[0].Instantiate<PlayerProjectile>();
			proj.Damage = this.Damage * proj.Multiplier;
			proj.CollisionLayer = 2;
			proj.Position = Position;
			var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
			proj.Rotation = angle;
			proj.Angle.X += angle;

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
				RandomNumberGenerator rng = new RandomNumberGenerator();
				TempAudio.PlayRandomPitch("grenade_launcher_shoot.wav", 0.2f, this);
				TempAudio.Play("crit_spawn.wav", this);
				ShotCooldownTimer.Start();
				CanShoot = false;
				var proj = Projectiles[0].Instantiate<PlayerProjectile>();
				proj.Damage = this.Damage * proj.Multiplier * 3;
				proj.CollisionLayer = 2;
				proj.Position = Position;
				proj.Hitsound = "crit_hit.wav";
				proj.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());
				var angle = rng.RandfRange(-SpreadAngle / 2, SpreadAngle / 2) / 90;
				proj.Rotation = angle;
				proj.Angle.X += angle;

				ProjectileContainer.AddChild(proj);
				return true;
			}
			return false;
		}
		else return Shoot(Position);
	}

	public override void LoadProjectiles()
	{
		Projectiles.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Pipe.tscn"));
	}
}
