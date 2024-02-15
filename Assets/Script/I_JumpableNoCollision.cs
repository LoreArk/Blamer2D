using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_JumpableNoCollision
{
    
    void IgnoreCollision(Collider2D collider, bool ignore);

    void MakeSolidByAxis(bool up, bool left);

    bool IsWalkableSurface();

    bool IsSolid();
}
