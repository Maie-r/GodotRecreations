using Godot;
using System;

public partial class RoamingProjectile : Area2D
{
	/*////////////////////////////////////////////////////////////////////////////////
	
		ROAMING PROJECTILE CLASS

		Purpose: Travel around, be killable, deal damage to the player

		What was Learned:
			Collisions should be treated almost exclusively by one side, otherwise
				get inconsistent results
			How to spawn things

	*/////////////////////////////////////////////////////////////////////////////////

	[Export]
	public float StartPosX = -1;
	[Export]
	public Vector2 angle = new Vector2(0, 1);
	[Export]
	public float speed = 200;
	[Export]
	public float speedVariance = 2;
	public float rotatespeed = 10;

	[Export]
	public float BaseHealth = 5;
	public float Health;

	[Signal]
	public delegate void PlayerCollisionEventHandler();
	[Signal]
	public delegate void BulletCollisionEventHandler();
	[Signal]
	public delegate void DeathEventHandler();

	RandomNumberGenerator rng = new RandomNumberGenerator();

	bool exploding = false;
	bool Donked = false;
	//bool Crited = false;

	public void Explode()
	{
		if (!exploding)
		{
			EmitSignal(SignalName.Death);
			TempAudio.PlayRandomPitch("hit.mp3", 0.2f, this);
			exploding = true;
			var sprite = GetNode<Sprite2D>("Sprite2D");
			sprite.Visible = false;
			//var hb = GetNode<CollisionShape2D>("Hitbox");
			this.CollisionMask = 0;
			this.CollisionLayer = 0;
			sprite.Visible = false;
			var part = GetNode<GpuParticles2D>("DeathExplosion");
			part.Emitting = true;
			part.Finished += () => this.QueueFree();
		}
	}

	/// <summary>
	/// returns true if dead
	/// </summary>
	public bool TakeDamage(float Damage)
	{
		//GD.Print("Damage taken: " + Damage);
		DamageNumber(Damage);
		Health -= Damage;
		if (Donked)
			{
				DoubleDonk();
			}
		if (Health <= 0)
		{
			Explode();
			return true;
		}
		else
		{
			
			this.rotatespeed = rng.RandfRange(-rotatespeed * 2, rotatespeed * 2);
			this.angle.Y *= 0.5f;
			var sprite = GetNode<Sprite2D>("Sprite2D");
			sprite.Material = sprite.Material.Duplicate() as ShaderMaterial;
			var e = (ShaderMaterial)sprite.Material;
			float hurtvalue = (Health / BaseHealth) + 0.3f;
			e.SetShaderParameter("Colorize", new Vector4(1, hurtvalue, hurtvalue, 1));
			TempAudio.PlayRandomPitch("sharp1.mp3", 0.2f, this);
			return false;
		}
	}

	public override void _Ready()
	{
		if (StartPosX < 0)
		{
			int width = (int)GetViewport().GetVisibleRect().Size[0] / 2;
			StartPosX = rng.RandfRange(-width, width);
		}
		if (angle.X == 0)
			angle = new Vector2(rng.RandfRange(-0.7f, 0.7f), 1);
		this.Position = new Vector2(StartPosX, -400);
		rotatespeed = rng.RandfRange(-60, 60);
		speed = rng.RandfRange(speed / speedVariance, speed * speedVariance);
		Health = BaseHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position += angle * speed * (float)delta;
		this.Rotate(rotatespeed / 1000);
	}

	private void _on_body_entered(Node2D body)
	{
		EmitSignal(SignalName.PlayerCollision);
		Explode();
	}

	private void _on_area_entered(Area2D area)
	{
		if (area is PlayerProjectile p)
		{
			p.OnCollision();
			if (p.IsCrit)
				Crit();
			if (Donked && p is CannonBall) { } // no chain damaging with a cannonball
			else if (!TakeDamage(p.Damage))

			EmitSignal(SignalName.BulletCollision);
		}
		else if (area.CollisionLayer == 9)
			this.QueueFree();
	}

	public void AddDonkStatus()
	{
		Donked = true;
		Timer tt = TempTimer.Get(0.2f);
		tt.Timeout += () => Donked = false;
		this.angle += new Vector2(0, -1);
	}

	void AddCritStatus()
	{
		Crit();
	}

	void DoubleDonk()
	{
		Donked = false;
		if (Health > 0)
			TakeDamage(5 + Health / 2);
		TempAudioGlobal.Play("doubledonk.wav", this);
		Donked = false;
		var donk = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/SlidingLabel.tscn").Instantiate<SlidingLabel>();
		//donk.Color = new Vector4(1, 1, 0, 1);
		donk.Text = "DONK! 2x";
		donk.Position = this.GlobalPosition + new Vector2(-20, -30);
		var label = donk.GetLabel();
		label.AddThemeFontOverride("font", ResourceLoader.Load<Font>("res://BulletHell/assets/TF2Build.ttf"));
		label.AddThemeFontSizeOverride("font_size", 30);
		label.AddThemeColorOverride("font_color", new Color(1, 1, 0, 1));
		label.AddThemeColorOverride("font_shadow_color", new Color("#683F96"));
		label.AddThemeColorOverride("font_outline_color", new Color("#683F96"));
		label.AddThemeConstantOverride("shadow_outline_size", 30);
		GetTree().Root.AddChild(donk);
	}

	void Crit()
	{
		//GD.Print("Crited!");
		var crit = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/SlidingLabel.tscn").Instantiate<SlidingLabel>();
		//Crited = false;
		//crit.Color = new Vector4(0, 1, 0, 1);
		crit.Text = "CRITICAL HIT!";
		crit.Position = this.GlobalPosition + new Vector2(-100, -50);
		var label = crit.GetLabel();
		label.AddThemeFontOverride("font", ResourceLoader.Load<Font>("res://BulletHell/assets/TF2.ttf"));
		label.AddThemeFontSizeOverride("font_size", 40);
		label.AddThemeColorOverride("font_color", new Color(0, 1, 0, 1));
		label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 1));
		label.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 1));
		GetTree().Root.AddChild(crit);
	}

	void DamageNumber(float Damage)
	{
		var SL = ResourceLoader.Load<PackedScene>("res://BulletHell/scenes/SlidingLabel.tscn").Instantiate<SlidingLabel>();
		var label = SL.GetLabel();
		label.AddThemeFontOverride("font", ResourceLoader.Load<Font>("res://BulletHell/assets/TF2.ttf"));
		//if (Crited) label.AddThemeFontSizeOverride("font_size", 40);
		label.AddThemeFontSizeOverride("font_size", 35);
		label.AddThemeColorOverride("font_color", new Color(1, 1, 0, 1));
		label.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 1));
		label.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 1));
		SL.Text = (-Damage).ToString("N0");
		SL.Position = this.GlobalPosition;
		GetTree().Root.AddChild(SL);
	}
}
