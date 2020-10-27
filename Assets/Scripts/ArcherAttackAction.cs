using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttackAction : Action
{
    public GameObject Arrow;

    public override void Perform(Unit p_From, Unit p_To, GameController p_Context)
    {
        GameObject cur_arrow = Instantiate<GameObject>(Arrow);
        cur_arrow.transform.position = p_From.transform.position;
        Vector3 to_position = p_To.CurTile.transform.position;
        Vector3 from_position = p_From.transform.position;
        if (to_position.x > from_position.x)
        {
            cur_arrow.transform.eulerAngles = new Vector3(0, 0, -90);
        }else if (to_position.x < from_position.x)
        {
            cur_arrow.transform.eulerAngles = new Vector3(0, 0, 90);
        }else if (to_position.y < from_position.y)
        {
            cur_arrow.transform.eulerAngles = new Vector3(0, 0, 180);
        }

        StartCoroutine(MoveArrow(cur_arrow.transform, to_position, 0.2f));
        p_To.OnAttack();

        int cur = p_To.GetStat("Health").CurrentValue;
        cur = cur - 1;
        p_To.SetStat("Health", cur);
    }

    private IEnumerator MoveArrow(Transform p_Target, Vector3 p_To, float p_Duration)
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

        Destroy(p_Target.gameObject);
    }
}
