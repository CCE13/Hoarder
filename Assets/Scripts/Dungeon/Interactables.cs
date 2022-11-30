using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Player;

public class Interactables : MonoBehaviour, Iinteractable
{
    [Header("Interact UI")]
    public GameObject interactUI;
    public Vector3 interactShowedSize;

    public bool interacted;

    private GameObject playerInteracting;

    public UnityEvent onOpen;

    private const float _timeToShow = 0.5f;

    public void CanInteract()
    {
        if (interacted) { return; }
        Show(interactUI, interactShowedSize);
        Debug.Log("Can Interact");
    }

    public void CannotInteract()
    {
        Hide(interactUI, Vector3.zero);
        Debug.Log("Left interaction zone");
    }


    public void Interacted(GameObject player)
    {
        if (interacted) return;
        if(playerInteracting != null) return;
        if (playerInteracting && playerInteracting != player) return;
        playerInteracting = player;
        onOpen.Invoke();
        interacted = true;
        Hide(interactUI, Vector3.zero);
    }

    #region InteractUI

    /// <summary>
    /// Shows the <paramref name="card"/> and scales it to the <paramref name="size"/>
    /// </summary>
    /// <param name="card"></param>
    /// <param name="size"></param>
    private void Show(GameObject card, Vector3 size)
    {
        StartCoroutine(Scaling(card, size));
    }


    /// <summary>
    /// Hides the <paramref name="card"/> and scales it to the <paramref name="size"/>
    /// </summary>
    /// <param name="card"></param>
    /// <param name="size"></param>
    private void Hide(GameObject card, Vector3 size)
    {
        StartCoroutine(Scaling(card, size));
    }

    private IEnumerator Scaling(GameObject card, Vector3 size)
    {
        for (float i = 0; i < _timeToShow; i += Time.fixedDeltaTime)
        {
            card.transform.localScale = new Vector3(
                Mathf.Lerp(card.transform.localScale.x, size.x, i),
                Mathf.Lerp(card.transform.localScale.y, size.y, i),
                Mathf.Lerp(card.transform.localScale.z, size.z, i));
            yield return null;
        }

        card.transform.localScale = size;

    }
    #endregion


}

