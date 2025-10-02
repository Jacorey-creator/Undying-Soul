using UnityEngine;

public interface IPossessable
{
    // Called when the player takes control
    void OnPossessed(GameObject possessor);

    // Called when the player leaves control
    void OnUnpossessed();

}
