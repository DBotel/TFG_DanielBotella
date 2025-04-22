using UnityEngine;

public class Wander : GAction
{
    private Vector3 targetPos;
    
    private void Start()
    {
        effects.Add("wander", 1);

        Vector3 randomOffset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        targetPos = transform.position + randomOffset;
        targetPos.y = 0;
        target = new GameObject("WanderTarget");
        target.transform.parent = gameObject.transform;
        target.tag = "WanderPoint";
        targetTag=target.tag;
        target.transform.position = targetPos;
    }
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        Destroy(target);
        Destroy(this);
        return true;
    }
}
