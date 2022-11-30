using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public interface Iinteractable
{
    void CanInteract();
    void Interacted(GameObject player);
    void CannotInteract();
}
