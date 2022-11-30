using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // Start is called before the first frame update

    public EnemyType enemyType;
    public List<GameObject> WayPoints;
    public int currentWayPoint = 0;
    public bool returnBack;

    private Rigidbody _rb;


    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        
        Patrol();
        
    }

    private void CheckWayPoint()
    {
        if(currentWayPoint == WayPoints.Count)
        {
            returnBack = true;
            currentWayPoint--;
        }
        if(currentWayPoint == 0)
        {
            returnBack = false;
        }

        if ((WayPoints[currentWayPoint].transform.position-transform.position).magnitude <= 1.5f)
        {
            if (returnBack)
            {
                currentWayPoint--;
            }
            else
            {
                currentWayPoint++;
            }
            
        }
    }

    public void Patrol()
    {
        CheckWayPoint();
        Vector3 positionToMoveTowards = WayPoints[currentWayPoint].transform.position - transform.position;
        Move(positionToMoveTowards.normalized);
    }

    public void Move(Vector3 positionToMoveTowards)
    {
        _rb.velocity = new Vector3(positionToMoveTowards.x, 0, positionToMoveTowards.z) * enemyType.speed * Time.fixedDeltaTime;
    }
}
