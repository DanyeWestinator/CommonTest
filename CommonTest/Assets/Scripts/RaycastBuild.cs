using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBuild : MonoBehaviour
{
    public GameObject cubePrefab;
    public Color cubeColor = Color.red;
    public bool timedDestroy = false;
    public float destroyTime = 5f;
    public Transform cubeParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    void CastRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            string name = hit.transform.name;
            if (name != "Floor" && name.StartsWith("Cube") == false)
            {
                print(hit.transform.name);
                return;
            }
            Vector3 cubePos = hit.point;
            cubePos.y += 0.5f;
            GameObject go = Instantiate(cubePrefab, cubePos, Quaternion.identity);
            go.GetComponent<Cube>().color = cubeColor;
            go.transform.parent = cubeParent;
            go.name = $"Cube {cubePos}";
            //go.transform.position = cubePos;
            if (timedDestroy)
                Destroy(go, destroyTime);
            Debug.DrawRay(transform.position, transform.forward, Color.red);
            //Debug.Break();
        }
    }
}
