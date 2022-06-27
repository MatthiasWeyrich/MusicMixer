using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRay : MonoBehaviour
{
    private Camera cam;
    void OnEnable()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Transform go = hit.collider.gameObject.transform.parent;
                if (go.TryGetComponent(out Node n))
                {
                    if (!go.TryGetComponent(out Start s))
                    {
                        if (n.Activated) n.Activated = false;
                        else n.Activated = true;
                    }
                }
            }
            
        }
    }
}