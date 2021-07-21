using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cube : NetworkBehaviour
{
    [HideInInspector]
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
        int frames = 0;
        while (true)
        {
            if (color == Color.clear)
            {
                yield return new WaitForEndOfFrame();
                frames++;
            }
            else
            {
                _color = color;
                break;
            }
        }
    }
    void SyncColor(Color old, Color c)
    {
        if (mr == null)
            mr = GetComponent<MeshRenderer>();
        mr.material.color = c;
    }

    [Command]
    public void CmdSetColor(Color c)
    {
        _color = c;
    }
}
