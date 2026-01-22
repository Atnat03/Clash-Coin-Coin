using UnityEngine;

public class TroopMulti : Troop
{
    public Animator[] animators;

    protected override void Update()
    {
        if (isFrozen)
            return;
    
        if (!enabled || !gridManager)
            return;
    
        target = Rescan();
    
        if (target && path.Count == 0)
        {
            FindPath(transform.position, target.position);
            lastTarget = target;
            pathRefreshTimer = PATH_REFRESH_TIME;
        }

        if (!target)
            return;
    

        foreach (var animator in animators)
        {
            animator.SetBool("Walk", isMoving && !isAttacking);
        }

        pathRefreshTimer -= Time.deltaTime;
        if (pathRefreshTimer <= 0f || target != lastTarget || path.Count == 0)
        {
            FindPath(transform.position, target.position);
            lastTarget = target;
            pathRefreshTimer = PATH_REFRESH_TIME;
        }
    
        if (isAttacking)
        {
            if (target)
            {
                Vector3 directionToTarget = (target.position - transform.position);
                directionToTarget.y = 0;
                RotateTowards(directionToTarget);
            }
            return;
        }
    
        if (target.TryGetComponent<ITargetable>(out var t))
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= RadiusAttack && t.CanBeAttacked)
            {

                isAttacking = true;

                foreach (var animator in animators)
                {
                    animator.ResetTrigger("Throw");
                }
                foreach (var animator in animators)
                {
                    animator.SetBool("Walk", false);
                }
                foreach (var animator in animators)
                {
                    animator.SetTrigger("Throw");
                }
            
                Vector3 directionToTarget = (target.position - transform.position);
                directionToTarget.y = 0;
                RotateTowards(directionToTarget);
                return;
            }
        }
    
        if (path.Count > 0)
        {
            Chase();
        }
    } 
}
