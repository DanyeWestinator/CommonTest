using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cube : NetworkBehaviour
{
    public Color color = Color.clear;
    private MeshRenderer mr;
    [SyncVar(hook = nameof(SyncColor))]
    private Color _color;

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(SetColor());
    }

    IEnumerator SetColor()
    {
        while (true)
        {
            if (color == Color.clear)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                _color = color;
                print("set color via coroutine");
                break;
            }
        }
    }
    void SyncColor(Color old, Color c)
    {
        if (mr == null)
            mr = GetComponent<MeshRenderer>();
        mr.material.color = c;
        print("set cube color via hook");
    }

    [Command]
    public void CmdSetColor(Color c)
    {
        _color = c;
        print("set cube color via command");
    }
}
