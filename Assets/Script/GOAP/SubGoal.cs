using System.Collections.Generic;

public class SubGoal
{
    public Dictionary<string, int> sGoals;
    public bool remove;

    public SubGoal(string key, int value, bool removeAfterAchieved)
    {
        sGoals = new Dictionary<string, int>();
        sGoals.Add(key, value);
        remove = removeAfterAchieved;
    }
}
