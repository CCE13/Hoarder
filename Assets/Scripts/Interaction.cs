using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class Interaction : MonoBehaviour
{
    Controls control;
    public bool isPlayer1;
    public bool isInteracting;

    public bool canInteract;
    private GameObject objectInteractingWith;

    private void Awake()
    {
        control = InputManager.instance.control;
        if(isPlayer1) control.SwordsMan.Interaction.started += ctx => Interact();
        else control.Ranger.Interaction.started += ctx => Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Iinteractable>() != null)
        {
            objectInteractingWith = other.gameObject;
            other.GetComponent<Iinteractable>().CanInteract();
        }
    }

    /// <summary>
    /// If the _player can interact, if the interaction button is pressed, interact with the delegated object.
    /// </summary>
    /// <param name="player"></param>
    private void Interact()
    {
        if (!objectInteractingWith) return;
        objectInteractingWith.GetComponent<Iinteractable>().Interacted(gameObject);
        isInteracting = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Iinteractable>() != null)
        {
            other.GetComponent<Iinteractable>().CannotInteract();
            objectInteractingWith = null;
        }
    }
}
