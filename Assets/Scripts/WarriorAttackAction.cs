using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAttackAction : Action
{ 
    public override void Perform(Unit p_From, Unit p_To, GameController p_Context)
    {

        // Move to
        StartCoroutine(MoveToAndBack(p_From.transform, p_To.CurTile.transform.position, 0.2f));

        p_To.OnAttack();

        int cur = p_To.GetStat("Health").CurrentValue;
        cur = cur - 1;
        p_To.SetStat("Health", cur);
    }

    private IEnumerator MoveToAndBack(Transform p_Target, Vector3 p_To, float p_Duration)
    {
        Vector3 original = p_Target.position;
        float time = 0;
        Vector3 start = p_Target.position;
        while (time <= p_Duration)
        {
            float t = Mathf.Clamp01(time / p_Duration);
            p_Target.transform.position = Vector3.Lerp(start, p_To, t);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        transform.position = original;
    }
}
