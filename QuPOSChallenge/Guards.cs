using System;

public static class Guards
{
  public static void GuardArgumentCheck(bool check, string message)
  {
    if (!check)
      throw new ArgumentException(message);
  }
}