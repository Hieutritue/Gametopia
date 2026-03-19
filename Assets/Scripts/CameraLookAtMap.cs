using UnityEngine;
using DefaultNamespace.Map;

public class CameraLookAtMap : MonoBehaviour
{
    public float heightOffset = 3f;
    public float distance = 8f;

    public float angleX = 45f;
    public float rotateSpeed = 10f;

    public float smoothTime = 0.3f;
    public float rotateSmooth = 5f;

    public float centerOffsetX = 0f; // ✅ NEW

    private float currentY;
    private Vector3 velocity;

    void Update()
    {
        currentY += rotateSpeed * Time.deltaTime;

        UpdateCamera();
    }

    void UpdateCamera()
    {
        MapManager map = MapManager.Instance;

        int width = map.Columns.GetLength(0);
        int depth = map.Columns.GetLength(1);

        float centerX = (width - 1) / 2f;
        float centerZ = (depth - 1) / 2f;

        float highestY = GetHighestY(map);

        // ✅ apply offset here
        Vector3 lookPoint = new Vector3(
            centerX + centerOffsetX,
            highestY,
            centerZ
        );

        Quaternion rot = Quaternion.Euler(
            angleX,
            currentY,
            0
        );

        Vector3 dir = rot * Vector3.back;

        Vector3 targetPos = lookPoint - dir * distance;

        targetPos.y = highestY + heightOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );

        Quaternion targetRot = Quaternion.LookRotation(
            lookPoint - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotateSmooth * Time.deltaTime
        );
    }

    float GetHighestY(MapManager map)
    {
        int width = map.Columns.GetLength(0);
        int depth = map.Columns.GetLength(1);

        float highestY = 0f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Column col = map.Columns[x, z];

                if (col.TopTile)
                {
                    float y = col.TopTile.transform.position.y;

                    if (y > highestY)
                        highestY = y;
                }
            }
        }

        return highestY;
    }
}