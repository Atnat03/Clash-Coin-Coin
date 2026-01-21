using UnityEngine;

public interface IPave
{
    public void Throw(Vector3 startPos, Vector3 targetPos
    , bool playerOneProperty, float damage, Collider mine);
}
