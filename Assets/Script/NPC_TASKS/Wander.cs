    using UnityEngine;

    public class Wander : GAction
    {
        private Vector3 targetPos;
        private void Start()
        {
            effects.Add("wander", 1);

            Vector3 randomOffset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            targetPos = transform.position + randomOffset;


            GameObject targetWonder;
            targetWonder = new GameObject("WanderTarget");
            targetWonder.transform.position = targetPos;
            target = targetWonder;
            target.transform.position = targetPos;
            target.gameObject.tag = "WanderPoint";
            target.transform.parent = null;

            targetTag = "WanderPoint";  
        }
        public override bool PrePerform()
        {
            agent.SetDestination(target.transform.position);
            runing = true;
            return true;
        }

        public override bool PostPerform()
        {
            Destroy(target);
            G_Agent.actions.Remove(this);
            Destroy(this);
            return true;
        }
    }
