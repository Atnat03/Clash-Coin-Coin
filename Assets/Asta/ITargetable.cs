using UnityEngine;

public interface ITargetable
{
    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage);

    bool CanBeAttacked { get; }

    public void GetPoisoned(float duration, float damage);
    bool IsMovementTarget { get; }
}
