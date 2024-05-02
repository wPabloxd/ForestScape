using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySight : MonoBehaviour
{
    [SerializeField] Vector3 sightSize = new Vector3(20f, 30f, 30f);
    [SerializeField] LayerMask layerMaskVisibles = Physics.DefaultRaycastLayers;
    [SerializeField] LayerMask layerMaskOccluders = Physics.DefaultRaycastLayers;
    [SerializeField] Vector2 sizeAngles = new Vector2(60f, 45f);
    [SerializeField] float frequency = 5;

    public List<IVisible> visiblesInSight = new();
    float lastSightCheckTime;

    private void Start()
    {
        lastSightCheckTime = Time.time;
        lastSightCheckTime += Random.Range(0f, 1f/ frequency);
    }
    private void Update()
    {
        if(Time.time - lastSightCheckTime > (1f/ frequency))
        {
            lastSightCheckTime += 1f / frequency;
            visiblesInSight.Clear();
            Collider[] colliders = Physics.OverlapBox(transform.position + (transform.forward * sightSize.z / 2f), sightSize / 2f, transform.rotation, layerMaskVisibles);
            foreach (Collider c in colliders)
            {
                //Debug.Log(c.gameObject.name + c.gameObject.name);
                Vector3 direction = c.transform.position - transform.position;
                //float horizontalAngle = Vector3.SignedAngle(transform.forward, direction, transform.up);
                //float verticalAngle = Vector3.SignedAngle(transform.forward, direction, transform.right);
                //if (Mathf.Abs(horizontalAngle) < sizeAngles.x && (Mathf.Abs(verticalAngle) < sizeAngles.y))
                //{
                    if (Physics.Raycast(transform.position, direction, out RaycastHit hit, direction.magnitude * 2f, layerMaskOccluders))
                    {
                        //Debug.Log((hit.collider, hit.collider) + " ME SUICIDO " + c.gameObject.name);
                        //Debug.DrawLine(transform.position, hit.point, Color.red);
                        if (c == hit.collider)
                        {
                            //Debug.Log("LLEGA");
                            IVisible visible = c.GetComponentInParent<IVisible>();
                            if (visible != null)
                            {
                                //Debug.Log(c.gameObject.name);
                                visiblesInSight.Add(visible);
                            }
                        }
                    }
                //}
            }
        }
    }
}