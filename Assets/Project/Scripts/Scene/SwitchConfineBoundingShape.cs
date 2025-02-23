using Cinemachine;
using UnityEngine;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void Start()
    {
        SwitchBoundingShape();
    }

    /// <summary>
    /// Switch the collider that Cinemachine uses to define the edges of the screen
    /// </summary>
    private void SwitchBoundingShape()
    {
        GameObject confinerObject = GameObject.FindGameObjectWithTag("BoundsConfiner");
        if (confinerObject == null)
        {
            Debug.LogError("BoundsConfiner object not found! Make sure it is tagged correctly.");
            return;
        }

        PolygonCollider2D polygonCollider2D = confinerObject.GetComponent<PolygonCollider2D>();
        if (polygonCollider2D == null)
        {
            Debug.LogError("PolygonCollider2D is missing from the BoundsConfiner object!");
            return;
        }

        CinemachineConfiner2D cinemachineConfiner = GetComponent<CinemachineConfiner2D>();
        if (cinemachineConfiner == null)
        {
            Debug.LogError("CinemachineConfiner2D component is missing on the Virtual Camera!");
            return;
        }

        cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
        cinemachineConfiner.InvalidateCache(); // Clears the cache to apply the new collider

        Debug.Log("CinemachineConfiner2D successfully updated with new bounds.");
    }
}
