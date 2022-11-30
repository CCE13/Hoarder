using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public LineRenderer lineOfSight;
    public Transform shootingPoint;
    public BoxCollider box;
    // Update is called once per frame
    private void Start()
    {
        box = GetComponent<BoxCollider>();
    }
    void Update() 
    {
        lineOfSight.transform.position = shootingPoint.position;
        lineOfSight.transform.rotation = transform.rotation;
        RaycastHit hit;
        Physics.Raycast(box.bounds.center, transform.forward, out hit, 100f);
        Debug.DrawRay(box.bounds.center, transform.forward * 100f, Color.cyan);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Arrow")){ return; }
            float magnitde = (hit.collider.transform.position - transform.position).magnitude;
            lineOfSight.SetPosition(1, new Vector3(0, 0, magnitde));
        }
        else
        {
            lineOfSight.SetPosition(1, new Vector3(0, 0, 100));
        }

    }
}
