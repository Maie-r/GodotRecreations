using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		PLAYER CLASS

		What was Learned:
			How Status Buffs work
			How to Emit Signals and use Groups
			That its best to centralize logic in its own class

	*/////////////////////////////////////////////////////////////////////////////////


	[Export]
	public float BaseSpeed = 300;
	public float Speed;

	[Export]
	public float BaseFireCoolDown = 1;
	public float FireCoolDown { get; set; }

	[Export]
	public int Lives = 5;

	//merely for testing buffs
	public float BaseDmg = 10;
	public float Damage;


	[Signal]
	public delegate void OnShootEventHandler(Vector2 Pos);
	[Signal]
	public delegate void OnHitEventHandler(int CurrentHealth);
	[Signal]
	public delegate void OnDeathEventHandler();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	List<Weapon> Weapons = new List<Weapon>();
	PackedScene[] Projectiles = new PackedScene[2];
	int _CurrentWeaponId = 0;
	public Weapon CurrentWeapon
	{
		get { return Weapons[_CurrentWeaponId]; }
	}
	public Marker2D BulletStartPos;
	Node2D ProjectileContainer;

	bool IsDead = false;
	bool IsShooting = false;

	[Export]
	public float RandomCritChance = 15;

	// DEBUG
	[Export]
	bool Godmode = false;
	[Export]
	bool AllWeapons = false;

	public Dictionary<Buff.BuffCategory, List<Buff>> Buffs = new Dictionary<Buff.BuffCategory, List<Buff>>();

	public override void _Ready()
	{
		BulletStartPos = GetNode<Marker2D>("BulletStartPos");
		LoadWeapons();
		LoadBuffContainer();
		RefreshBuffs();
		FireCoolDown = BaseFireCoolDown;
		//AudioPlayer = GetNode<AudioStreamPlayer2D>("Sfx");
		//AudioPlayer.Stream = (AudioStream)ResourceLoader.Load(audiofolder + "demoman_painsevere01.mp3");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsDead)
		{
			Vector2 velocity = Velocity;

			if (Input.IsActionJustPressed("C"))
				SwitchWeapons();
			/*if (Input.IsActionPressed("Z"))
				Speed = BaseSpeed * 1.5f;
			else if (Speed > BaseSpeed)
				Speed = BaseSpeed;*/

			if (Input.IsActionPressed("Shoot"))
			{
				IsShooting = true;
				Shoot();
			}
			else if (IsShooting)
			{
				IsShooting = false;
				StoppedShooting();
			}

			Vector2 direction = Input.GetVector("left", "right", "up", "down");
			if (direction != Vector2.Zero)
			{
				velocity.X = direction.X * Speed * (float)delta * 100;
				velocity.Y = direction.Y * Speed * (float)delta * 100;
			}
			else
			{
				//velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed) * (float)delta;
				//velocity.Y = Mathf.MoveToward(velocity.Y, 0, Speed) * (float)delta;
				velocity.X = 0;
				velocity.Y = 0;
			}
			Velocity = velocity;
			MoveAndSlide();
		}
	}

	public void Shoot()
	{
		if (rng.RandfRange(0, 100) < RandomCritChance)
		{
			if (CurrentWeapon.Shoot(true))
				EmitSignal(SignalName.OnShoot, BulletStartPos.GlobalPosition);
		}
		else
		{
			if (CurrentWeapon.Shoot())
				EmitSignal(SignalName.OnShoot, BulletStartPos.GlobalPosition);
		}
	}

	public void StoppedShooting()
	{
		if (CurrentWeapon is IChargeWeapon c)
		{
			if (rng.RandfRange(0, 100) < RandomCritChance)
			{
			if (c.ChargeStop(true))
				EmitSignal(SignalName.OnShoot, BulletStartPos.GlobalPosition);
			}
			else
			{
			if (c.ChargeStop())
				EmitSignal(SignalName.OnShoot, BulletStartPos.GlobalPosition);
			}
		}
		
	}

	public void SwitchWeapons()
	{
		if (CurrentWeapon is IChargeWeapon e && e.IsCharging()) return;
		CurrentWeapon.Toggle();
		_CurrentWeaponId++;
		_CurrentWeaponId = _CurrentWeaponId % Weapons.Count;
		GD.Print("Current Weapon: " + _CurrentWeaponId);
		CurrentWeapon.Toggle();
		TempAudio.Play("E half life.wav", this);
	}

	public void AddWeapon(Weapon w)
	{
		Weapons.Add(w);
		GetNode<Node2D>("Weapons").AddChild(w);
		TempAudio.Play("gunpickup2.wav", this);
		if (CurrentWeapon is IChargeWeapon e && e.IsCharging()) return;
		CurrentWeapon.Toggle();
		_CurrentWeaponId = Weapons.Count-1;
		CurrentWeapon.Toggle();
	}

	public bool HasWeapon(Weapon w)
	{
		foreach (Weapon wpn in Weapons)
		{
			if (wpn.Name == w.Name) return true;
		}
		return false;
	}

	public void TakeDamage(int amount = 1)
	{
		if (!Godmode)
		{
			TempAudio.Play("hit.wav", this);
			Lives -= amount;
			EmitSignal(SignalName.OnHit, Lives);
			GetTree().CallGroup("UI", "SetLives", Lives);
			RandomNumberGenerator rng = new RandomNumberGenerator();
			if (Lives <= 0)
			{
				TempAudio.Play("crit_received1.wav", this);
				DemoDie();
				//AudioPlayer.Stream = (AudioStream)ResourceLoader.Load(audiofolder + "demoman_paincrticialdeath01.mp3");
				EmitSignal(SignalName.OnDeath);
				GetNode<Node2D>("Sprites").Visible = false;
				//GetNode<CollisionShape2D>("hurtbox").Disabled = true;
				this.CollisionLayer = 32; //this is because setting the hurtbox to disabled isn't working??
				IsDead = true;
			}
			else
				DemoHurt();
		}

	}

	public void Reset()
	{
		Lives = 5;
		GetNode<Node2D>("Sprites").Visible = true;
		IsDead = false;
		//AudioPlayer.Stream = (AudioStream)ResourceLoader.Load(audiofolder + "demoman_painsevere01.mp3");
		GetNode<CollisionShape2D>("hurtbox").Disabled = false;
		FireCoolDown = BaseFireCoolDown;
		this.CollisionLayer = 2;
		Buffs.Clear();
		if (!AllWeapons)
		{
			while(_CurrentWeaponId > 0)
				SwitchWeapons();
			LoadWeapons();
			/*var gun = Weapons.First();
			Weapons.Clear();
			Weapons.Add(gun);*/
		}
		GD.Print($"WEaponcount: {Weapons.Count}");
		LoadBuffContainer();
		SwitchWeapons();
		RefreshBuffs();
		GetTree().CallGroup("UI", "SetLives", Lives);
	}

	public void AddPowerup(Buff p)
	{
		AddChild(p);
		Buffs[p.Category].Add(p);
		if (p.Category is Buff.BuffCategory.Crit)
		{
			var og = RandomCritChance;
			var critfx = CritEffect.GetCritEffect().Instantiate<Node2D>();
			this.Material = critfx.Material;
			GetNode<Node2D>("Weapons").UseParentMaterial = true;
			AddChild(critfx);
			RandomCritChance = 100;
			p.StartTimer();
			p.Timeout = () =>
			{
				this.Material = null;
				GetNode<Node2D>("Weapons").UseParentMaterial = false;
				RandomCritChance = og;
				RemovePowerup(p);
				critfx.QueueFree();
				this.UseParentMaterial = false;
			};
		}
		else if (p.Category is Buff.BuffCategory.Uber)
		{
			var og = Godmode;
			var critfx = CritEffect.GetCritEffect().Instantiate<Node2D>();
			this.Material = critfx.Material;
			GetNode<Node2D>("Sprites").UseParentMaterial = true;
			AddChild(critfx);
			Godmode = true;
			p.StartTimer();
			p.Timeout = () =>
			{
				this.Material = null;
				GetNode<Node2D>("Sprites").UseParentMaterial = false;
				Godmode = og;
				RemovePowerup(p);
				critfx.QueueFree();
			};
		}
		else
		{
			RefreshBuffs();
			p.StartTimer();
			p.Timeout = () => RemovePowerup(p);
		}

		GetTree().CallGroup("UI", "SetLives", Lives);
		TempAudio.Play("gunpickup2.wav", this);
		DemoLaugh();
	}

	void RemovePowerup(Buff b, Timer t)
	{
		t.QueueFree();
		RemovePowerup(b);
	}

	public void RemovePowerup(Buff b)
	{
		Buffs[b.Category].Remove(b);
		RefreshBuffs();
		//GD.Print("Timeout " + b.Category);
		if (b.Category is not Buff.BuffCategory.Health)
		{
			if (rng.Randf() > 0.5) DemoSad(); 
			else DemoDrunk();
		}
		b.QueueFree();
	}

	public void RefreshBuffs()
	{
		FireCoolDown = BaseFireCoolDown * Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.FireRate], Buff.BuffAddType.Multiplicative)
						+ Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.FireRate]);
		if (FireCoolDown < 0.05) FireCoolDown = 0.05f;

		Weapons.ForEach((Weapon w) => w.UpdateFireRate(FireCoolDown));

		Speed = BaseSpeed * Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.Speed], Buff.BuffAddType.Multiplicative)
						+ Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Speed]);
		if (Speed < 0) Speed = 0;
		else if (Speed > 3000) Speed = 3000;

		Damage = BaseDmg * Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.Damage], Buff.BuffAddType.Multiplicative)
						+ Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Damage]);


		Lives += (int)Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Health]);
		if (Lives > 5) Lives = 5;
		/*
		GD.Print($"Fire rate: ({BaseFireCoolDown} * {Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.FireRate], Buff.BuffAddType.Multiplicative)}) + {Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.FireRate])} = {FireCoolDown}");
		GD.Print($"Speed: ({BaseSpeed} * {Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.Speed], Buff.BuffAddType.Multiplicative)}) + {Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Speed])} = {Speed}");
		GD.Print($"Damage: ({BaseDmg} * {Buff.GetBuffsMultiplier(Buffs[Buff.BuffCategory.Damage], Buff.BuffAddType.Multiplicative)}) + {Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Damage])} = {Damage}");
		GD.Print($"Health: {Lives-Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Health])} + {Buff.GetFlatBuffs(Buffs[Buff.BuffCategory.Health])} = {Lives}");
		*/
	}
}

public partial class Player : CharacterBody2D // SOUND EFFECT PLAYING
{
	RandomNumberGenerator rng = new RandomNumberGenerator();

	void DemoLaugh()
	{
		// this one is a special case since the file names line up with the rng
		TempAudio.Play($"demoman_laughevil0{rng.RandiRange(1, 5)}.mp3", this);
	}

	void DemoHurt()
	{
		switch (rng.RandiRange(1, 2))
		{
			case 1:
				TempAudio.Play("demoman_painsevere01.mp3", this);
				break;
			case 2:
				TempAudio.Play("demoman_painsharp02.mp3", this);
				break;
			case 3:
				TempAudio.Play("demoman_painsharp03.mp3", this);
				break;
			case 4:
				TempAudio.Play("demoman_painsharp01.mp3", this);
				break;
			case 5:
				TempAudio.Play("demoman_painsharp04.mp3", this);
				break;
			case 6:
				TempAudio.Play("demoman_painsharp05.mp3", this);
				break;
		}
	}

	void DemoDie()
	{
		switch (rng.RandiRange(1, 2))
		{
			case 1:
				TempAudio.Play("demoman_paincrticialdeath01.mp3", this);
				break;
			case 2:
				TempAudio.Play("demoman_paincrticialdeath05.mp3", this);
				break;
		}
	}

	void DemoSad()
	{
		switch (rng.RandiRange(1, 3))
		{
			case 1:
				TempAudio.Play("demoman_negativevocalization02.mp3", this);
				break;
			case 2:
				TempAudio.Play("demoman_negativevocalization04.mp3", this);
				break;
			case 3:
				TempAudio.Play("demoman_negativevocalization06.mp3", this);
				break;
		}
	}

	void DemoDrunk()
	{
		switch (rng.RandiRange(1, 4))
		{
			case 1:
				TempAudio.Play("demoman_gibberish01.mp3", this);
				break;
			case 2:
				TempAudio.Play("demoman_gibberish06.mp3", this);
				break;
			case 3:
				TempAudio.Play("demoman_gibberish09.mp3", this);
				break;
			case 4:
				TempAudio.Play("demoman_gibberish10.mp3", this);
				break;
		}
	}
}

public partial class Player : CharacterBody2D // RESOURCE LOADING
{
	void LoadWeapons()
	{
		Weapons.Clear();
		var e = GetNode<Node2D>("Weapons");
		int i = AllWeapons ? -999 : 0;
		foreach (Weapon w in e.GetChildren())
		{
			if (i > 0) { e.RemoveChild(w); }
			else
			{
				w.SetPlayer(this);
				Weapons.Add(w);
				i++;
			}
		}
		GD.Print(Weapons.Count + " Weapons loaded!");
		//Projectiles[0] = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Bullet.tscn");
		//Projectiles[1] = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/PlayerProjectiles/Triple.tscn");
	}

	void LoadBuffContainer()
	{
		Buffs.Add(Buff.BuffCategory.Damage, new List<Buff>());
		Buffs.Add(Buff.BuffCategory.FireRate, new List<Buff>());
		Buffs.Add(Buff.BuffCategory.Speed, new List<Buff>());
		Buffs.Add(Buff.BuffCategory.Health, new List<Buff>());
		Buffs.Add(Buff.BuffCategory.Crit, new List<Buff>());
		Buffs.Add(Buff.BuffCategory.Uber, new List<Buff>());
	}
}
