using System;
using Godot;

public partial class Madeline : CharacterBody2D, IPushable
{
    [Export]
    public float JumpForce = 50;

    [Export]
    public float BaseSpeed = 200;

    public int MaxJumpCount = 1;
    public int JumpCount = 1;

    public int MaxDashCount = 1;
    public int DashCount = 1;
    public Timer DashCooldownTimer;
    public bool CanDash = true;

    public Timer GroundedSafetyTimer;

    public Timer CoyoteTimer;

    public float Gravity = 50;


    public override void _Ready()
    {
        SetTimers();
    }

    public override void _Process(double delta)
    {
       Vector2 vel = Velocity;
       vel.Y += Gravity * (float)delta;

       /*
       if (OnGround)
          SetState(States.Grounded)
       else
       {
          if (JumpCount == MaxJumpCount)
             CoyoteTimer.Start();
          SetState(States.Falling);
       }

       var Axis = Input.GetVector("left","right","up","down");
       if (Axis.X == Vector.Zero)
       {
          vel.X *= 0.9f;
       }
       else 
       {
          vel.X += BaseSpeed * Axis.X * delta;
       }
       */

       if (Input.ActionJustPressed("dash"))
          ApplyImpulse(Axis * 200);
    }

    public override void ApplyImpulse(Vector2 impulse) 
    {
       Velocity += impulse;
    }
}

// STATES
public partial class Madeline : CharacterBody2D
{
   public enum States{
      Falling,
      Grounded
   };

   public bool ChangeState(State s)
   {
      if (CurrentState == s) return;
      CurrentState = s;
      s.OnSet();
   }
}

// SETUPS
public partial class Madeline : CharacterBody2D
{
    SetTimers()
    {
        DashCooldownTimer = new Timer()
        {
            Oneshot = true,
            WaitTime = 1,
            TimeOut += () => CanDash = true;
        };
        GroundedSafetyTimer = new Timer()
        {
            Oneshot = true,
            WaitTime = 3
        };
        CoyoteTimer = new Timer(){
            Oneshot = true,
            WaitTime = 1,
            TimeOut += () => JumpCount = 0
        };

        AddChild(GroundedSafetyTimer);
        AddChild(CoyoteTimer);
    }
}

public interface IPushable
{
    public void ApplyImpulse(Vector2 impulse);
}