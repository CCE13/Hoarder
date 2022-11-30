using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    public float aimAssistSize;
    public GameObject shootingPoint;
    private GameObject currentTarget;
    private Controls control;

    // Start is called before the first frame update

    //A sphere cast is shot forward, if theere are collisions found, aim the _player to the first enemy the sphere cast collided with.

    public void Awake()
    {
        control = InputManager.instance.control;
        //control.Ranger.LockOn.performed += ctx => LockOn();
    }
    public void CheckAim()
    {
        for (int i = 0; i < 50; i++)
        {
            Vector3 posToCast = shootingPoint.transform.position + transform.forward * i;
            Collider[] collisions = Physics.OverlapSphere(posToCast, aimAssistSize);
            //ensures that the _player only aims towards enemies and not other objects.
            if(collisions.Length > 0)
            {
                foreach (Collider enemy in collisions)
                {
                    if (enemy.GetComponent<EnemyManager>() != null)
                    {
                        currentTarget = enemy.gameObject;
                        currentTarget.transform.Find("Targetted").gameObject.SetActive(true);
                        break;
                    }
                }
            }

        }
        
    }

    public void LockOn()
    {
        CheckAim();
        //if (currentTarget == null) { Debug.Log("No taret");  return; }
        transform.LookAt(new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z));
    }

}
