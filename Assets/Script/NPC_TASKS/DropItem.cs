using UnityEngine;

public class DropItem : GAction
{
    public GameObject item;

    private void Start()
    {
        target = item;
        targetTag = item.tag;
        effects.Add("droppingItem", 1);
    }
    public override bool PrePerform()
    {
       
        return true;
    }

    public override bool PostPerform()
    {
        item.SetActive(true);
        G_Agent.inventory.RemoveItem(item);
        G_Agent.actions.Remove(this);
        Destroy(this);
        return true;
    }
}
