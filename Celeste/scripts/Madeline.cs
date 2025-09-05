using System;
using Godot;

public partial class Madeline : CharacterBody2D
{
    [Export]
    public float JumpForce = 50;

    [Export]
    public float BaseSpeed = 200;

    public int MaxJumpCount = 1;
    public int JumpCount = 1;

    public int MaxDashCount = 1;
    public int DashCount = 1;

    public GroundedSafetyTimer;


    public override void _Ready()
    {
        SetTimers();
    }

    public override void _Process(double delta)
    {
       /*
       if (OnGround)
          SetState(States.Grounded)
       
       */
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
        GroundedSafetyTimer = new Timer()
        {
            Oneshot = true,
            WaitTime = 3
        };

        AddChild(GroundedSafetyTimer);
    }
}
