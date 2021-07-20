using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DanePlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public List<Color> colors = new List<Color>();
    [SyncVar(hook = nameof(SyncColor))]
    Color color;

    public Color currentColor => color;

    [SyncVar]
    int playerNum;

    static NetworkManagerTest nm;

    private List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>();

    [SerializeField] private Animator _animator = null;

    public override void OnStartClient()
    {
        if (nm == null)
            nm = NetworkManagerTest.instance;
        base.OnStartClient();
        if (isLocalPlayer)
        {
            playerNum = nm.numPlayers;
        }
        color = colors[Random.Range(0, colors.Count)];
        gameObject.name = $"Player {playerNum}";
        GetComponentInChildren<Text>().text = gameObject.name;
    }

    void SyncColor(Color old, Color newColor)
    {
        if (!meshRenderers.Any())
        {
            meshRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
        }
        print("set color via hook");
        meshRenderers.ForEach(mr=> mr.material.color = newColor);
    }

    [Command]
    void CmdSetColor(Color c)
    {
        color = c;
        print("set color via command");
    }

    
    [Command]
    void CmdSpawnCube(Vector3 pos, Color c)
    {
        GameObject cubePrefab = nm.spawnPrefabs.Find(go => go.name == "Cube");
        GameObject cube = Instantiate(cubePrefab);
        pos.y = 0f;
        cube.transform.position = pos;
        cube.GetComponent<Cube>().color = currentColor;
        NetworkServer.Spawn(cube);
    }

    private void OnDestroy()
    {
        meshRenderers.ForEach(Destroy);
    }
    private void Update()
    {
        if (!isLocalPlayer) return;
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
            
        var movement = new Vector3(x, 0f, y).normalized;
            
        transform.position += movement * speed * Time.deltaTime;

        if (movement.magnitude >= .2f)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
            
        _animator.SetBool("isWalking", movement.magnitude >= .2f);
            
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Color c = colors[Random.Range(0, colors.Count)];
            CmdSetColor(c);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdSpawnCube(transform.position, currentColor);
        }
    }
}
