using UnityEngine;

public class BezierPathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    private float t = 0f;
    private Quaternion defaultRotation = Quaternion.Euler(-90f, 0f, 0f);

    private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private void Update()
    {
        if (waypoints.Length == 4)
        {
            t += Time.deltaTime * speed * 0.01f;

            if (t > 1f)
            {
                t -= 1f;
            }

            Vector3 p0 = waypoints[0].position;
            Vector3 p1 = waypoints[1].position;
            Vector3 p2 = waypoints[2].position;
            Vector3 p3 = waypoints[3].position;

            Vector3 position = GetPoint(p0, p1, p2, p3, t);
            transform.position = position;

            float nextT = t + 0.001f;
            if (nextT > 1f) nextT -= 1f;
            Vector3 nextPosition = GetPoint(p0, p1, p2, p3, nextT);
            Vector3 direction = transform.position - nextPosition;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
                transform.rotation = targetRotation * defaultRotation;
            }
        }
    }
}
