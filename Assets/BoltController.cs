using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoltController : MonoBehaviour
{
    public static bool IsFiring
    {
        get
        {
            return instance.CoroutinesRunning > 0;
        }
    }

    private static BoltController instance;

    private int CoroutinesRunning = 0;

    private void Awake()
    {
        instance = this;
    }

    public static void FireBolts(int x, int y, bool[,] touched)
    {
        instance.CoroutinesRunning++;
        SFXManager.PlayBoltSound();
        instance.StartCoroutine(instance.FireBoltsInternal(x, y, touched));
    }

    private IEnumerator FireBoltsInternal(int x, int y, bool[,] touched)
    {
        Level.Active.Tiles[x, y].SuppressPowerPlant = true;

        RectTransform source = (RectTransform)FindObjectsOfType<tileButton>()
            .Where(tb => tb.x == x && tb.y == y)
            .Single()
            .transform;

        IEnumerable<Vector2> deltas = Enumerable.Range(0, 30)
            .Where(i => touched[i % 6, i / 6] && Level.Active.Tiles[i % 6, i / 6].ContainsBuilding)
            .Select<int, Vector2>(i =>
            {
                int dx = -(x - (i % 6));
                int dy = y - (i / 6);
                return new Vector2(source.sizeDelta.x * dx, source.sizeDelta.y * dy);
            });

        GameObject boltPrefab = Resources.Load<GameObject>("Bolt");

        List<GameObject> bolts = new List<GameObject>();

        foreach (Vector2 delta in deltas)
        {
            GameObject bolt = Instantiate<GameObject>(boltPrefab);
            bolt.transform.SetParent(source, false);
            LineRenderer boltRenderer = bolt.GetComponent<LineRenderer>();
            bolts.Add(bolt);

            float magnitude = delta.magnitude;
            int segments = Mathf.CeilToInt(magnitude * 0.1f);
            Vector2 displacementPerIndex = delta / segments;
            Vector2 perlinStart = new Vector2(Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
            // total perlin displacement ~5
            Vector2 perlinDisplacementPerIndex = delta * 5 / magnitude / segments;
            Vector2 normal = new Vector2(delta.y, -delta.x).normalized * magnitude * 0.3f;

            boltRenderer.positionCount = segments + 1;
            boltRenderer.SetPosition(0, Vector3.zero);
            for (int i = 0; i < segments - 1; i++)
            {
                float lerpFactor = 1f;
                if (i < 3)
                {
                    lerpFactor = i * (1f / 3f);
                }
                Vector2 perlinSampleLocation = perlinStart + perlinDisplacementPerIndex * i;
                float perlinSample =(0.5f - Mathf.PerlinNoise(perlinSampleLocation.x, perlinSampleLocation.y)) * 2f;
                boltRenderer.SetPosition(i + 1, displacementPerIndex * i + normal * perlinSample * lerpFactor);
            }
            boltRenderer.SetPosition(segments, delta);
        }

        yield return new WaitForSeconds(0.4f);

        Level.Active.Tiles[x, y].SuppressPowerPlant = false;
        foreach (GameObject bolt in bolts)
        {
            Destroy(bolt);
        }
        CoroutinesRunning--;
        yield break;
    }
}