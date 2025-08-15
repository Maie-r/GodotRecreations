using Godot;
using System;
using System.Collections.Generic;

public partial class BulletHell : Node2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		BULLETHELL CLASS

		Purpose: Reponsible for main game logic: Spawning objects, and calling Groups based on signals and events

		What was Learned:
			Balancing difficulty is actually very hard haha
			Contrary to the tutorial I followed, I think having main game logic outside of the interacted's scene
				or class is pretty bad, ruins the modularity, like if I had a "Shoot" method here, the player 
				wouldn't be able to shoot in any other scene
			Godot has a RandomNumberGenerator!
			GROUPS! they let you call a scene group and a method within without getting the node itself (which can
				be kinda icky). Don't know how it works if the group is NOT within the scene tho

	*/////////////////////////////////////////////////////////////////////////////////

	PackedScene[] Projectiles = new PackedScene[2];
	PackedScene[] PowerUps = new PackedScene[2];
	List<PackedScene> Weapons = new List<PackedScene>();
	int spawnedweaponcount = 0;
	Buff[] BuffPool;
	Node2D SpawnableContainer;

	RandomNumberGenerator rng = new RandomNumberGenerator();

	PlayerInfo PlayerInfo;

	int difficultyquota = 0;

	int difficulty = 1;

	public override void _Ready()
	{
		//Engine.TimeScale = 10;
		Player player = GetNode<Player>("Player");
		Projectiles[0] = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/RoamingProjectile.tscn");
		Projectiles[1] = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Bullet.tscn");
		PowerUps[0] = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PowerUp.tscn");

		Weapons.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerWeapons/Stock.tscn"));
		Weapons.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerWeapons/Shotgun.tscn"));
		Weapons.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerWeapons/Triple_Gun.tscn"));
		Weapons.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerWeapons/Locken.tscn"));
		Weapons.Add(ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerWeapons/Loose.tscn"));

		SpawnableContainer = GetNode<Node2D>("Spawnables");
		var ui = GetNode<UI>("UI");
		PlayerInfo = (PlayerInfo)GetNode<CanvasLayer>("PlayerInfo");
		PlayerInfo.SetPlayer(player);
		ui.PlayerRef = player;
		BuffPool = [
			new Buff
			{
				Category = Buff.BuffCategory.FireRate,
				Duration = 15,
				Multiplier = -0.4f
			},
			new Buff
			{
				Category = Buff.BuffCategory.Damage,
				Duration = 15,
				Multiplier = 0.4f
			},
			new Buff
			{
				Category = Buff.BuffCategory.Speed,
				Duration = 15,
				Multiplier = 0.1f
			},
			new Buff
			{
				Category = Buff.BuffCategory.Health,
				Duration = 0.01f,
				Flat = 1
			},
			new Buff
			{
				Category = Buff.BuffCategory.Crit,
				Duration = 8
			},
			new Buff
			{
				Category = Buff.BuffCategory.Uber,
				Duration = 8
			}
		];
	}

	public override void _Process(double delta) // I put these here because they are all debug
	{
		if (Input.IsActionJustPressed("D"))
			ToggleCameraZoom();

		if (Input.IsActionJustPressed("Debug_Menu"))
			PlayerInfo.ToggleVis();

	}

	private void _on_roaming_projectile_timer_timeout()
	{
		try
		{
			difficultyquota++;
			//GD.Print($"{difficultyquota} > {20 + (10 * difficulty)}");
			if (difficultyquota > 20 + (10 * difficulty))
			{
				var timer = GetNode<Timer>("Spawnables/RoamingProjectiles/RoamingProjectileTimer");
				var time = timer.WaitTime * 0.95;
				if (time > 0.1) // stops at level 28 (in theory)
				{
					timer.WaitTime = time;
				}
				difficulty++;
				difficultyquota = 0;
				GD.Print("Difficulty level " + difficulty);
				//GetTree().CallGroup("UI", "AddScore", 25);
				SpawnWeapon();
				var poweruptimer = GetNode<Timer>("Spawnables/Powerups/PowerupTimer");
				var time2 = poweruptimer.WaitTime * 0.95;
				if (time > 0.1) poweruptimer.WaitTime = time2; // this is absolutely broken lmao
			}
			var temp = (RoamingProjectile)Projectiles[0].Instantiate();
			var speedboost = 1 + difficulty / 20 + (difficulty / (difficulty * difficulty)) / 10;
			if (speedboost > 3) speedboost = 3;
			var sizeboost = rng.RandfRange(0.8f, 2.5f);
			temp.speed *= speedboost / (sizeboost);
			temp.BaseHealth += temp.BaseHealth * difficulty / 3 * sizeboost;
			temp.Scale *= sizeboost;
			temp.Connect(RoamingProjectile.SignalName.PlayerCollision, Callable.From(PlayerHit));
			temp.Connect(RoamingProjectile.SignalName.BulletCollision, Callable.From(RoamProjHit));
			temp.Connect(RoamingProjectile.SignalName.Death, Callable.From(RoamProjKill));
			SpawnableContainer.GetChild(0).AddChild(temp);
		}
		catch (Exception e)
		{
			GD.PushError(e.Message);
		}

	}

	private void _on_player_on_shoot(Vector2 pos) // hell nah
	{
		/*
		var temp = (Bullet)Projectiles[1].Instantiate();
		//GD.Print(temp.CollisionLayer);
		temp.CollisionLayer = 2;
		SpawnableContainer.GetChild(1).AddChild(temp);
		temp.Position = pos;
		var p = (Player)(GetNode<CharacterBody2D>("Player"));
		temp.Damage = p.Damage;//*/
	}

	private void _on_powerup_timer_timeout()
	{
		PowerUp temp = (PowerUp)PowerUps[0].Instantiate();
		temp.Pickup = RandomBuff();
		//GD.Print("Spawning Powerup: " + temp.buff.Category);
		temp.Connect(PowerUp.SignalName.Collected, Callable.From((PowerUp p) => PlayerPower((Buff)p.Pickup)));
		var b = (Buff)temp.Pickup;
		Sprite2D sprite;
		switch (b.Category) // Preferribly these should be within the Buff class itself, but I didn't want to bloat it
		{
			case Buff.BuffCategory.Health:
				sprite = temp.GetNode<Sprite2D>("Sprite2D");
				sprite.Texture = ResourceLoader.Load<Texture2D>("res://BulletHell/assets/healthsmall.png");
				sprite.RegionEnabled = false;
				sprite.Scale = new Vector2(0.15f, 0.15f);
				TempAudio.Play("demoman_medic03.mp3", this);
				break;
			case Buff.BuffCategory.FireRate:
				sprite = temp.GetNode<Sprite2D>("Sprite2D");
				sprite.Texture = ResourceLoader.Load<Texture2D>("res://BulletHell/assets/scrumpy.png");
				sprite.RegionEnabled = false;
				sprite.Scale = new Vector2(0.4f, 0.4f);
				TempAudio.Play("demoman_specialcompleted02.mp3", this);
				break;
			case Buff.BuffCategory.Damage:
				sprite = temp.GetNode<Sprite2D>("Sprite2D");
				sprite.Texture = ResourceLoader.Load<Texture2D>("res://BulletHell/assets/Damage.png");
				sprite.RegionEnabled = false;
				sprite.Scale = new Vector2(1, 1);
				TempAudio.Play("spawn_item.wav", this);
				break;
			case Buff.BuffCategory.Speed:
				sprite = temp.GetNode<Sprite2D>("Sprite2D");
				sprite.Texture = ResourceLoader.Load<Texture2D>("res://BulletHell/assets/Speed.png");
				sprite.RegionEnabled = false;
				sprite.Scale = new Vector2(1, 1);
				TempAudio.Play("spawn_item.wav", this);
				break;
			default:
				TempAudio.Play("spawn_item.wav", this);
				break;
		}

		SpawnableContainer.GetChild(2).AddChild(temp);
	}

	private void SpawnWeapon()
	{
		if (spawnedweaponcount >= Weapons.Count) return;
		PowerUp temp = (PowerUp)PowerUps[0].Instantiate();
		temp.GetChild<Sprite2D>(0).Visible = false;
		Weapon w = RandomWeapon();
		if (w == null) return;
		w.StartActive = false;
		temp.Pickup = w;
		w.SetPlayer(GetNode<Player>("Player"));
		temp.Connect(PowerUp.SignalName.Collected, Callable.From((PowerUp p) => PlayerWeapon(w)));
		var sprites = (Node2D)w.GetChild<Node2D>(0).Duplicate();
		sprites.Visible = true;
		sprites.Scale = new Vector2(0.2f, 0.2f);
		sprites.Position = new Vector2(-20, 10);
		temp.AddChild(sprites);
		SpawnableContainer.GetChild(2).AddChild(temp);
	}

	private void _on_player_on_death()
	{
		GetTree().CallGroup("UI", "Finish");
	}
	private void PlayerHit()
	{
		//GetTree().CallGroup("UI", "AddLives", -1);
		GetTree().CallGroup("Player", "TakeDamage", 1);
		GetTree().CallGroup("UI", "AddScore", -20);
	}

	public void PlayerPower(Buff p)
	{
		GetTree().CallGroup("Player", "AddPowerup", p);
		GetTree().CallGroup("UI", "AddScore", 5);
	}

	public void PlayerWeapon(Weapon w)
	{
		GetTree().CallGroup("Player", "AddWeapon", w);
		spawnedweaponcount++;
	}

	private void RoamProjHit()
	{
		GetTree().CallGroup("UI", "AddScore", 1);
	}

	private void RoamProjKill()
	{
		GetTree().CallGroup("UI", "AddScore", 10);
	}

	private void _on_ui_restart()
	{
		GetTree().CallGroup("Player", "Reset");
		this.Reset();
	}

	void ToggleCameraZoom()
	{
		var cam = GetNode<Camera2D>("Camera2D");
		if (cam.Zoom[0] >= 1)
			cam.Zoom = new Vector2(0.3f, 0.3f);
		else
			cam.Zoom = new Vector2(1, 1);
	}

	Buff RandomBuff()
	{
		var id = rng.RandiRange(0, BuffPool.Length - 1);
		//GD.Print(id);
		var buff = BuffPool[id];
		var copy = new Buff
		{
			Category = buff.Category,
			Duration = buff.Duration,
			Flat = buff.Flat,
			Multiplier = buff.Multiplier
		};
		return copy;
	}

	Weapon RandomWeapon()
	{
		var wpn = Weapons[rng.RandiRange(0, Weapons.Count - 1)];
		var copy = wpn.Instantiate<Weapon>();
		var player = GetNode<Player>("Player");
		int i = 0;
		while (player.HasWeapon(copy)) // this is prob a terrible way to only get unique weapons
		{
			i++;
			if (i < 10)
			{
				wpn = Weapons[rng.RandiRange(0, Weapons.Count - 1)];
				copy = wpn.Instantiate<Weapon>();
			}
			else
			{
				int j = 0;
				while (player.HasWeapon(copy))
				{
					if (j++ > Weapons.Count)
					{
						spawnedweaponcount = Weapons.Count;
						return null;
					}
					wpn = Weapons[rng.RandiRange(0, Weapons.Count - 1)];
					copy = wpn.Instantiate<Weapon>();
				}

			}

		}
		GD.Print($"That took {i} tries");

		return copy;
	}

	public void Reset()
	{
		foreach (var container in SpawnableContainer.GetChildren())
			foreach (var child in container.GetChildren())
				if (child is not Timer)
					child.QueueFree();

		difficulty = 1;
		difficultyquota = 0;
		GetNode<CharacterBody2D>("Player").Position = new Vector2(0, 260);
		GetNode<Timer>("Spawnables/RoamingProjectiles/RoamingProjectileTimer").WaitTime = 0.4;
	}
}
