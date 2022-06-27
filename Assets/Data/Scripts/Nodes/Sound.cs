using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sound : Intermediary
{
    public AudioSource source { get; set; }
    public SoundData data;
    public AudioManager am;

    Dictionary<int,Parameter> parameterList;

    protected override void OnEnable()
    {
        base.OnEnable();
        data = new SoundData();
        parameterList = new Dictionary<int, Parameter>();

    }
    public override void addListeners()
    {
        Button.onClick.AddListener(handleDeleteButton);
        canvas.enabled = false;
    }
    public override void Interact()
    {
        if (!Activated)
        {
            Invokation();
            return;
        }
        if(!paused) { 
            data.prepareData(parameterList);
            am.playSound(source,data);
            data.clearDictionary();
            r.material.color = Color.magenta;
        }
        StartCoroutine("waitUntil");
    }
    IEnumerator waitUntil(){
        while(paused) yield return null;
        Invoke("Invokation",source.clip.length-0.09f);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        // getting the offset from the center of the node to the actual position the node was clicked
        mouseOffset = transform.position - GetMouseWorldPos();
            // if left mouse button, we draw a new line
        if(Input.GetMouseButton(0)) {
            skeleton.CreateLine(GetMouseWorldPos());
            skeleton.currentLine.from = id;
            drawing = true;
        }
            // else we move the line and thus need to destroy all connection -> invoking the event and destroying all outgoing lines
        else if(Input.GetMouseButton(1))
        {
            drawing = false;
            DeleteLinesDueToMovement?.Invoke(id);
            parameterList.Clear();
            for (int i = outgoingLines.Count - 1; i >= 0; i--)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        // nothing happens if we're not drawing
        // if we're drawing however,
        // we check if any object is below the mouse courser at the moment of release
        // if that's the case, we check whether its a valid node
        // and then add it to the lines "to" field
        if (drawing)
        {
            List<GameObject> hoveredObjects = eventData.hovered;
            foreach (GameObject g in hoveredObjects)
            {
                if (g.TryGetComponent(out Intermediary i))
                {
                    Node n = g.GetComponent<Node>();
                    nm.addChild(n.id);
                    skeleton.currentLine.to = n.id;
                    outgoingLines.Add(skeleton.currentLine);
                    return;
                }
                else {
                    if(g.TryGetComponent(out Parameter param)){
                        Node n = g.GetComponent<Node>();
                        nm.addChild(n.id);
                        skeleton.currentLine.to = n.id;
                        outgoingLines.Add(skeleton.currentLine);
                        parameterList.Add(param.id,param);
                        return;
                    }
                }
            }
            // if no valid game object was hit, we destroy the line instead
            if(skeleton.currentLine.gameObject!=null) Destroy(skeleton.currentLine.gameObject);
        }
        drawing = false;
    }

    // Applying the changes of the Parameter to the source
    /// TODO
    /// -> Event called by the Parameter if value was changed by the user via the ui


    public override void RemoveInvolvedLines(int id)
    {
        // this is called by the NodeManager if it got a notice that a node has moved
        // all nodes will check their outgoing lines and remove and destroy all that have the moved nodes id as their destination
        if (id == this.id) return;
            // If a parameter was removed, we need to revert its changes to our source.
        if(parameterList.ContainsKey(id)){
            parameterList.Remove(id);
        }
        for (int i = outgoingLines.Count-1; i >=0; i--)
        {
            if (outgoingLines[i].to == id)
            {
                LineInteraction ln = outgoingLines[i];
                outgoingLines.Remove(ln);
                Destroy(ln.gameObject);
            }
        }
    }
    private void Invokation(){
        if(Activated) r.material.color = defaultC;
        nm.notifyChildren();
    }
    
    public override void onStopCommand()
    {
        source.Pause();
        paused = true;
    }

    public override void OnContinueCommand()
    {
        paused = false;
    }

    public override void OnStartCommand()
    {
        paused = false;
    }
}
