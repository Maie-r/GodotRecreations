using Godot;
using System;

public partial class Triple_Gun : Weapon
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		TRIPLE GUN CLASS

		Purpose: It's the regular gun but 3x, 3x

		What was Learned:
			Multiple projectiles are not that difficult to make, unless you want the hits
				to have a cooldown

		What was NOT implemented:
			Update the shooting position to where the player actually is instead of where he was
			(pretty simple fix actually but idc)


	*/////////////////////////////////////////////////////////////////////////////////
	Timer InnerTimer;

	public override bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			ShotCooldownTimer.Start();
			CanShoot = false;
			ShootThree(Position, 0);
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
				ShootThree(true, Position, 0);
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

	public override void LoadAdditional()
	{
		/*InnerTimer = new Timer();
		InnerTimer.WaitTime = 0.3f;
		AddChild(InnerTimer);*/
	}

	void ShootThree(Vector2 Position, int o)
	{
		if (o > 2) return;
		Sht(Position);
		Timer tt = new Timer();
		tt.Autostart = true;
		tt.WaitTime = 0.1f;
		tt.Timeout += () => { ShootThree(Position, o + 1); tt.QueueFree(); };
		AddChild(tt);
	}

	void ShootThree(bool Crit, Vector2 Position, int o)
	{
		if (!Crit) ShootThree(Position, o);
		else
		{
			if (o > 2) return;
			Sht(true, Position);
			Timer tt = new Timer();
			tt.Autostart = true;
			tt.WaitTime = 0.1f;
			tt.Timeout += () => { ShootThree(true, Position, o + 1); tt.QueueFree(); };
			AddChild(tt);
		}

	}

	void Sht(Vector2 Position)
	{
		TempAudio.PlayRandomPitch("shoot.mp3", 0.2f, this);
		for (int i = -1; i < 2; i++)
		{
			var proj = Projectiles[0].Instantiate<Area2D>();
			proj.CollisionLayer = 2;
			var bp = new Vector2(Position.X, Position.Y);
			bp.X += i * 30;
			proj.Position = bp;

			if (proj is PlayerProjectile p)
			{
				p.Damage = Damage * 0.5f;
				if (i == 0) p.BaseSpeed *= 1.2f;
			}
			else GD.Print("Ermm what the heck");

			ProjectileContainer.AddChild(proj);
		}
	}

	void Sht(bool Crit, Vector2 Position)
	{
		if (!Crit) Sht(Position);
		else
		{
			TempAudio.PlayRandomPitch("shoot.mp3", 0.2f, this);
			TempAudio.Play("crit_spawn.wav", this);
			for (int i = -1; i < 2; i++)
			{
				var proj = Projectiles[0].Instantiate<Area2D>();
				proj.CollisionLayer = 2;
				var bp = new Vector2(Position.X, Position.Y);
				bp.X += i * 30;
				proj.Position = bp;

				if (proj is PlayerProjectile p)
				{
					p.Damage = Damage * 0.5f * 3;
					p.Hitsound = "crit_hit.wav";
					p.AddChild(CritEffect.GetCritEffect().Instantiate<Node2D>());
					if (i == 0) p.BaseSpeed *= 1.2f;
				}
				else GD.Print("Ermm what the heck");

				ProjectileContainer.AddChild(proj);
			}
		}
	}
}
