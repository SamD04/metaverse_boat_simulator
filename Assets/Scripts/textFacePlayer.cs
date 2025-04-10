using UnityEngine;

public class textFacePlayer : MonoBehaviour
{
    public Transform player;

    void LateUpdate()
    {
        if (player == null)
        {
            if (Camera.main != null)
                player = Camera.main.transform;
            else
                return;
        }

        transform.LookAt(player);

        transform.Rotate(0, 180f, 0);
    }
}
