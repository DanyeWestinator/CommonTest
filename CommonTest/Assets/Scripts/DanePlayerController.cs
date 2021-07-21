using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DanePlayerController : NetworkBehaviour
{
    public float speed = 5f;
    public List<Color> colors = new List<Color>();
    [SyncVar(hook = nameof(SyncColor))] private Color color;

    public Color currentColor => color;

    [SyncVar] private int playerNum;

    private static NetworkManagerTest nm;

    private List<SkinnedMeshRenderer> meshRenderers = new List<SkinnedMeshRenderer>();

    [SerializeField] private Animator _animator = null;
    [SerializeField] private GameObject model = null;


    public override void OnStartClient()
    {
        if (nm == null)
            nm = NetworkManagerTest.instance;
        base.OnStartClient();
        if (isLocalPlayer)
        {
            playerNum = nm.numPlayers;
            foreach (Transform child in model.transform)
            {
                child.gameObject.SetActive(false);
            }
            
            Camera.main.gameObject.SetActive(false);
        }
        else
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam)
            {
                Destroy(cam);
            }
        }
        
        color = colors[Random.Range(0, colors.Count)];
        gameObject.name = $"Player {playerNum}";
        //GetComponentInChildren<Text>().text = gameObject.name;
    }

    private void SyncColor(Color old, Color newColor)
    {
        if (!meshRenderers.Any())
        {
            meshRenderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>(true));
        }
        print("set color via hook");
        meshRenderers.ForEach(mr=> mr.material.color = newColor);
    }

    [Command]
    private void CmdSetColor(Color c)
    {
        color = c;
        print("set color via command");
    }

    [Command]
    private void CmdSpawnCube(Vector3 pos, Color c)
    {
        var cubePrefab = nm.spawnPrefabs.Find(go => go.name == "Cube");
        var cube = Instantiate(cubePrefab);
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
        
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
            
        var movement = new Vector3(x, 0f, y).normalized;
        
        var velocityMagnitude = GetComponent<CharacterController>().velocity.magnitude;
        //Debug.Log($"velocity: {velocityMagnitude}");
        //_animator.SetBool("isWalking", velocityMagnitude >= .2f);
            
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //var c = colors[Random.Range(0, colors.Count)];
            //CmdSetColor(c);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //CmdSpawnCube(transform.position, currentColor);
        }
    }
}
