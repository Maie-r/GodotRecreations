using System;

public abstract class State
{
   public string Name;
   public StateCategory category;

   public abstract OnSet();
}
 public static class StateCategory{
   public enum Get{
      FootState
   };
}