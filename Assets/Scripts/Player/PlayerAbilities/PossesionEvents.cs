using System;
using UnityEngine;

public static class PossessionEvents
{
    // Called when ghost wants to possess a target
    public static Action<GameObject> OnAttemptPossess;

    // Called when ghost wants to unpossess current target
    public static Action OnAttemptUnpossess;
}
