using UnityEngine;

public class DespawnTrigger : MonoBehaviour
{
    [SerializeField] private LevelGenerator levelGenerator;
    void Start()
    {
        if (levelGenerator == null)
        {
            levelGenerator = GameObject.FindObjectOfType<LevelGenerator>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MapSegment segment = collision.GetComponent<MapSegment>();
        if (segment != null)
        {
            levelGenerator.DespawnSegment(segment);
        }
    }
}
