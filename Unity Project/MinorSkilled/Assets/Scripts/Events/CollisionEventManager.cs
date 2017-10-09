using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

public class CollisionEventManager : MonoBehaviour
{
    public List<Collision3DEvent> events = new List<Collision3DEvent>();

    private void OnTriggerEnter(Collider other)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onTriggerEnter, other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onTriggerStay, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onTriggerExit, other);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onCollisionEnter, col.collider);
        }
    }

    private void OnCollisionStay(Collision col)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onCollisionStay, col.collider);
        }
    }

    private void OnCollisionExit(Collision col)
    {
        foreach (Collision3DEvent collision3DEvent in events)
        {
            collision3DEvent.HandleEvent(collision3DEvent.onCollisionExit, col.collider);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(CollisionEventManager),true)]
[CanEditMultipleObjects]
public class CollisionEventManagerEditor : Editor
{
    private CollisionEventManager manager;
    private List<Editor> cachedEditors = new List<Editor>();
    private List<bool> showEvents = new List<bool>();
    private UnityEvent garbageCollector = new UnityEvent();


    [InitializeOnLoadMethod]
    static void Awake()
    {
        
    }

    public override void OnInspectorGUI()
    {
        if (manager == null) manager = (CollisionEventManager)target;
        GUI.color = Color.green;
        if (GUILayout.Button("Add event"))
            manager.events.Add(CreateInstance<Collision3DEvent>());
        GUI.color = Color.white;

        for (int i = 0; i < manager.events.Count; i++)
        {
			if (cachedEditors.Count < manager.events.Count) cachedEditors.Add(ScriptableObject.CreateInstance<Editor>());
            if (showEvents.Count < manager.events.Count) showEvents.Add(false);
            showEvents[i] = EditorGUILayout.Foldout(showEvents[i], "Event " + (i+1));
            if (showEvents[i])
            {
                GUI.color = Color.red;
                if (GUILayout.Button("Delete")) RemoveEvent(manager.events[i]);
                GUI.color = Color.white;
                Editor tempEditor = cachedEditors[i];
                CreateCachedEditor(manager.events[i], typeof(EventTriggerEditor), ref tempEditor);
                if (tempEditor != null) tempEditor.OnInspectorGUI();
                cachedEditors[i] = tempEditor;
            }
        }

        //Garbage collector? overkill? yes. Unecessary? maybe. Better alternatives? Definitely.
        if (garbageCollector != null)
        {
            garbageCollector.Invoke();
            garbageCollector.RemoveAllListeners();
        }

        EditorUtility.SetDirty(target);
    }

    private void RemoveEvent(Collision3DEvent collision3DEvent)
    {
        garbageCollector.AddListener(delegate { manager.events.Remove(collision3DEvent); });
    }
}
#endif
