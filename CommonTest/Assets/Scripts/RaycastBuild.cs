using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class RaycastBuild : NetworkBehaviour
{
    private GameObject cubePrefab;
    public Color cubeColor = Color.red;
    public bool timedDestroy = false;
    public float destroyTime = 5f;
    public Transform cubeParent;

    public Transform crosshair;
    
    private RaycastHit hit;
    public Image colorGUI;

    private List<Color> colors;
    private int colorIndex = 0;

    private static NetworkManagerTest nm;

    
    HashSet<Vector3> positions = new HashSet<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        if (cubeParent == null)
        {
            cubeParent = GameObject.Find("CubeParent").transform;
        }
        if (nm == null)
        {
            nm = NetworkManagerTest.instance;
            cubePrefab = nm.spawnPrefabs.Find(go => go.name == "Cube");
        }
        colors = GetComponent<DanePlayerController>().colors;
        colorGUI.color = cubeColor;
        if (isLocalPlayer == false)
        {
            crosshair.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer == false)
            return;
        DrawCrosshair();
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
        if (Input.GetMouseButtonDown(1))
        {
            CastRay(true);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextColor();
        }
    }

    private void NextColor()
    {
        colorIndex++;
        if (colorIndex >= colors.Count)
            colorIndex = 0;
        cubeColor = colors[colorIndex];
        colorGUI.color = cubeColor;

    }

    /// <summary>
    /// Spawns the cube on the server side
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="c"></param>
    [Command]
    void CmdSpawnCube(Vector3 pos, Color c)
    {
        pos = Vector3Int.RoundToInt(pos);
        if (positions.Contains(pos))
            return;
        cubePrefab = nm.spawnPrefabs.Find(x => x.name == "Cube");
        GameObject go = Instantiate(cubePrefab, pos, Quaternion.identity);
        go.GetComponent<Cube>().color = c;
        go.transform.parent = cubeParent;
        go.name = $"Cube {pos}";

        NetworkServer.Spawn(go);
        positions.Add(pos);

        if (timedDestroy)
            Destroy(go, destroyTime);
    }

    [Command]
    void CmdDeleteCube(GameObject cube)
    {
        NetworkServer.Destroy(cube);
    }

    /// <summary>
    /// draws the crosshair where the cursor is pointing
    /// </summary>
    void DrawCrosshair()
    {
        //RaycastHit hit;
        Vector3 forward = transform.GetChild(0).transform.forward;
        if (Physics.Raycast(transform.position, forward, out this.hit))
        {
            string name = hit.transform.name;
            crosshair.gameObject.SetActive(true);
            if (name != "Floor" && name.StartsWith("Cube") == false)
                return;
            Vector3 pos = hit.point;
            pos.y += 0.5f;
            //pos = Vector3Int.RoundToInt(pos);
            crosshair.position = pos;
            crosshair.eulerAngles = hit.normal;
        }
        else
        {
            crosshair.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// checks if the raycast is valid, then spawns a cube
    /// </summary>
    void CastRay(bool destroy = false)
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, transform.forward, out hit))
        if (this.hit.collider != null)
        {
            string name = hit.transform.name;
            if (name != "Floor" && name.StartsWith("Cube") == false)
            {
                print(hit.transform.name);
                return;
            }
            Vector3 cubePos = hit.point;
            cubePos.y += 0.5f;
            if (destroy == false)
                CmdSpawnCube(cubePos, cubeColor);
            else if (destroy && name.StartsWith("Cube"))
                CmdDeleteCube(this.hit.transform.gameObject);
            
        }
    }
}
