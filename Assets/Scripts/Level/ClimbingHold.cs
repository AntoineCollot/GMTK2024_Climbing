using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingHold : MonoBehaviour
{
    public Vector3 grabbedPosition { get; private set; }
    public bool isGrabbed { get; private set; }
    public bool isHovered { get; private set; }

    [SerializeField] protected Renderer highlight;

    //Add a little offset for the hand doesn't clip
    public const float HAND_Y_OFFSET = 0.002f;
    const float OUTLINE_WIDTH = 0.02f;

    protected virtual void Start()
    {
        highlight.gameObject.SetActive(false);
    }

    public virtual float DistanceFrom(Vector3 position)
    {
        return Vector3.Distance(position, GetGrabPosition(position));
    }

    public virtual Vector3 GetGrabPosition(Vector3 from)
    {
        return transform.position + Vector3.up * HAND_Y_OFFSET;
    }

    public virtual void OnHoverEnter()
    {
        isHovered = true;
        highlight.gameObject.SetActive(true);
    }

    public virtual void OnHoverExit()
    {
        isHovered = false;
        highlight.gameObject.SetActive(false);
    }

    public virtual void OnGrabbed(Vector3 from)
    {
        isGrabbed = true;
        grabbedPosition = GetGrabPosition(from);
        highlight.gameObject.SetActive(false);
    }

    public virtual void OnReleased()
    {
        isGrabbed=false;
    }
}
