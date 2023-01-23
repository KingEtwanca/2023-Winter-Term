using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorToSkeleton : MonoBehaviour
{
    public GameObject Skeleton;
    public void StartAttack() {
        Skeleton.GetComponent<Skeleton_AI>().startAttack();
    }

    public void EndAttack() {
        Skeleton.GetComponent<Skeleton_AI>().endAttack();
    }

    public void startThrust() {
        Skeleton.GetComponent<Skeleton_AI>().startThrust();
    }
    public void endThrust()
    {
        Skeleton.GetComponent<Skeleton_AI>().endThrust();
    }

    public void AHitbox() {
        Skeleton.GetComponent<Skeleton_AI>().ActivateHitbox();
    }

    public void DHitbox()
    {
        Skeleton.GetComponent<Skeleton_AI>().DeactivateHitbox();
    }

    public void Die() {
        Skeleton.GetComponent<Skeleton_AI>().Die();
    }
}
