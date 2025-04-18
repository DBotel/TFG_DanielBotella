using UnityEngine;

public class Patient : GAgent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("choppedTree", 1, true);
        goals.Add(s1,3);
    }

    
}
