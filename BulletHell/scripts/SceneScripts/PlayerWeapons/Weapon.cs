using Godot;
using System;
using System.Collections.Generic;

public partial class Weapon : Node2D, IPickup
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		WEAPON CLASS

		Purpose: Represent a Shooting Equipment with it's own shooting logic and projectiles

		What was Learned:
			No, not all buffs are as easy and "Hard Coding" can be inescapable. At first these were
				going to be buffs, but I realised it isn't that simple
			It is actually very powerful to define a class for your Main attack type

		What was NOT implemented:
			Actual bullet count and magazines, reloading and shit like that

	*/////////////////////////////////////////////////////////////////////////////////

	Player PlayerRef;
	public float Damage { get { return PlayerRef.Damage; } }
	[Export]
	public bool StartActive { get; set; } = true;
	bool Active;
	public List<PackedScene> Projectiles { get; set; } = new List<PackedScene>();
	public Node2D ProjectileContainer { get; set; }
	public Marker2D SpawnPos;

	[Export]
	public float BaseFireCoolDown { get; set; } = 1;
	public float FireCoolDown { get { return (float)ShotCooldownTimer.WaitTime; } set { ShotCooldownTimer.WaitTime = value; } }
	public Timer ShotCooldownTimer { get; set; }
	public float ShotCooldownMultiplier { get { return PlayerRef.FireCoolDown; } }
	public bool CanShoot { get; set; } = true;

	public override void _Ready()
	{
		LoadStuff();
	}

	public virtual bool Shoot()
	{
		try
		{
			return Shoot(SpawnPos.GlobalPosition);
		}
		catch 
		{
			GD.PushWarning($"Weapon {this.Name} does not have a player reference! ");
			return Shoot(this.GlobalPosition);
		}
	}

	public virtual bool Shoot(bool RandomCrit)
	{
		if (RandomCrit)
		{
			try
			{
				return Shoot(RandomCrit, SpawnPos.GlobalPosition);
			}
			catch
			{
				GD.Print(SpawnPos == null);
				GD.PushWarning($"Weapon {this.Name} does not have a player reference! ");
				return Shoot(RandomCrit, this.GlobalPosition);
			}
		}
		else return Shoot();
	}

	public virtual bool Shoot(Vector2 Position)
	{
		if (CanShoot)
		{
			ShotCooldownTimer.Start();
			CanShoot = false;
			var proj = Projectiles[0].Instantiate<Area2D>();
			proj.Position = Position;

			if (proj is PlayerProjectile p)
				p.Damage = Damage;
			else GD.Print("Ermm what the heck");

			ProjectileContainer.AddChild(proj);

			return true;
		}
		return false;
	}

	public virtual bool Shoot(bool RandomCrit, Vector2 Position)
	{
		if (RandomCrit)
		{
			if (CanShoot)
			{
				TempAudio.Play("crit_spawn.wav", this);
				ShotCooldownTimer.Start();
				CanShoot = false;
				var proj = Projectiles[0].Instantiate<PlayerProjectile>();
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

	public void LoadProjectileContainer()
	{
		ProjectileContainer = GetTree().Root.FindChild("PlayerProjectiles", true, false) as Node2D;
		if (ProjectileContainer == null)
		{
			ProjectileContainer = new Node2D { Name = "PlayerProjectiles" };
			GetTree().Root.AddChild(ProjectileContainer);
		}
	}

	public void LoadShotCooldownTimer()
	{
		ShotCooldownTimer = new Timer();
		ShotCooldownTimer.WaitTime = BaseFireCoolDown;
		ShotCooldownTimer.OneShot = true;
		this.AddChild(ShotCooldownTimer);
		ShotCooldownTimer.Timeout += () => CanShoot = true;
	}

	public virtual void LoadProjectiles()
	{

	}

	public virtual void SetPlayer(Player player)
	{
		this.PlayerRef = player;
		SpawnPos = player.BulletStartPos;
	}

	public virtual void LoadStuff()
	{
		Active = StartActive;
		GetNode<Node2D>("Sprites").Visible = Active;
		LoadProjectileContainer();
		LoadShotCooldownTimer();
		LoadProjectiles();
		LoadAdditional();
		//if (PlayerRef == null)
		//GD.PushError("Player not set for a weapon!! Set it with the 'SetPlayer' method when loading");
	}

	public virtual void UpdateFireRate(float Coef)
	{
		FireCoolDown = BaseFireCoolDown * Coef;
	}

	public virtual void Toggle()
	{
		Active = !Active;
		this.Visible = Active;
		GetNode<Node2D>("Sprites").Visible = Active;
	}

	/// <summary>
	/// Does nothing on its own, override to load additional resources at _Ready
	/// </summary>
	public virtual void LoadAdditional() { }
}

public static class CritEffect
{
	public static PackedScene GetCritEffect()
	{
		return ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/crit_effect.tscn");
	}
}
