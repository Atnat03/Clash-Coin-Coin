using UnityEngine;

public interface ITargetable
{
    public bool playerOneProperty { get; set; }
    public void TakeDamage(float damage);

    public void GetPoisoned(float duration, float damage);
}
