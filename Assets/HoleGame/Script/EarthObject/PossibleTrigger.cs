using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PossibleTrigger : MonoBehaviour
{
    private int currentLevel = 0;

    private readonly HashSet<FallingObject> objectsInTrigger = new();
    public void SetLevel(int level)
    {
        currentLevel=level;
        RefreshTriggerOutlineOnLevelChange();
    }

    private void OnTriggerEnter(Collider other)
    {

        FallingObject fallObj = other.GetComponent<FallingObject>();
        if (fallObj != null)
        {
            objectsInTrigger.Add(fallObj);

            TryUpdateOutline(fallObj); // 최초 진입 시도 처리
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FallingObject fallObj = other.GetComponent<FallingObject>();
        if (fallObj != null)
        {
            objectsInTrigger.Remove(fallObj);
        }
    }

    private void RefreshTriggerOutlineOnLevelChange()
    {
        foreach (FallingObject obj in objectsInTrigger.ToList())
        {
            if (obj == null || obj.Equals(null))
            {
                objectsInTrigger.Remove(obj);
                continue;
            }

            TryUpdateOutline(obj);
        }
    }

    private void TryUpdateOutline(FallingObject obj)
    {
        if (obj.GetShapeEnum() == ShapeEnum.boomb) return;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) return;

        Material mat = renderer.material;

        if (currentLevel >= obj.ObjectMass) 
        {
            mat.SetFloat("_OutlineWidth", 0.5f);
           
        }
        else
        {
            mat.SetFloat("_OutlineWidth", 0f);
        }
    }
}
