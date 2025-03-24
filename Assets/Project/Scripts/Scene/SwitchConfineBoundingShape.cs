using Cinemachine;
using UnityEngine;
using System.Collections;

public class SwitchConfineBoundingShape : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(TrySwitchBoundingShape());    }

    /// <summary>
    /// Switch the collider that Cinemachine uses to define the edges of the screen
    /// </summary>
   private IEnumerator TrySwitchBoundingShape(float timeout = 5f, float retryInterval = 0.1f)
{
    float timer = 0f;

    while (timer < timeout)
    {
        GameObject confinerObject = GameObject.FindGameObjectWithTag("BoundsConfiner");

        if (confinerObject != null)
        {
            PolygonCollider2D polygonCollider2D = confinerObject.GetComponent<PolygonCollider2D>();
            if (polygonCollider2D == null)
            {
                Debug.LogError("PolygonCollider2D is missing from the BoundsConfiner object!");
                yield break;
            }

            CinemachineConfiner2D cinemachineConfiner = GetComponent<CinemachineConfiner2D>();
            if (cinemachineConfiner == null)
            {
                Debug.LogError("CinemachineConfiner2D component is missing on the Virtual Camera!");
                yield break;
            }

            cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;
            cinemachineConfiner.InvalidateCache();

            Debug.Log("✅ CinemachineConfiner2D successfully updated with new bounds.");
            yield break;
        }

        timer += retryInterval;
        yield return new WaitForSeconds(retryInterval);
    }

    Debug.LogWarning("⚠️ Failed to find BoundsConfiner within timeout.");
}

}
