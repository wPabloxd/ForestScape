using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class HeadLevelJump : MonoBehaviour
{
    IEntityAnimable entityAnimable;
    [SerializeField] ParentConstraint parentConstraint;
    void Awake()
    {
        entityAnimable = GetComponent<IEntityAnimable>();
    }

    // Update is called once per frame
    void Update()
    {
        HeadLevel(entityAnimable.GetVerticalVelocity());
    }
    void HeadLevel(float verticalVelocity)
    {
        //Debug.Log(verticalVelocity);
        if (verticalVelocity != 0)
        {
            //Debug.Log("SALTO");
            ConstraintSource source0 = parentConstraint.GetSource(0);
            source0.weight = 0;
            parentConstraint.SetSource(0, source0);
            ConstraintSource source1 = parentConstraint.GetSource(1);
            source1.weight = 1;
            parentConstraint.SetSource(1, source1);
        }
        else
        {
            //Debug.Log("NOSALTO");
            ConstraintSource source0 = parentConstraint.GetSource(0);
            source0.weight = 1;
            parentConstraint.SetSource(0, source0);
            ConstraintSource source1 = parentConstraint.GetSource(1);
            source1.weight = 0.2f;
            parentConstraint.SetSource(1, source1);
        }

    }
}
