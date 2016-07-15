using UnityEngine;
using System.Collections;

public class SpawnableBase : MonoBehaviour
{
    public Renderer Renderer;

    protected SpawnableManagerBase _parentManager;

    public void InitSpawnable(SpawnableManagerBase parentManager)
    {
        _parentManager = parentManager;

        Deactivate();
    }

    public virtual void Activate(Vector3 spawnPos)
    {
        transform.position = spawnPos;

        transform.parent = null;

        SetRendererActive(true);

        gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        transform.parent = _parentManager.transform;

        _parentManager.AddToDeactiveList(this);

        gameObject.SetActive(false);
    }

    protected void SetRendererActive(bool isActive)
    {
        Renderer.enabled = isActive;
    }
}
