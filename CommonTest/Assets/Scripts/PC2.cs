using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Player controller 2, a test script for moving a cube
/// </summary>
public class PC2 : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;
    private Camera _camera;
    private bool local => GetComponentInParent<EmptyPlayer>().isLocalPlayer;
    public string goName;

    [SyncVar(hook = nameof(SyncName))]
    private string _name;

    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
        print(local);
        if (local)
        {
            _camera.gameObject.SetActive(true);
            print("set camera to active");
        }
        else
        {
            _camera.gameObject.SetActive(false);
            print("turned camera off");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (local)
        {
            Vector3 angles = transform.eulerAngles;
            angles.y += (Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime);
            transform.eulerAngles = angles;
            Vector3 pos = transform.localPosition;
            pos.z += (Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
            transform.localPosition = pos;
            
        }
    }

    void SyncName(string old, string name)
    {
        gameObject.name = name;
    }
}
