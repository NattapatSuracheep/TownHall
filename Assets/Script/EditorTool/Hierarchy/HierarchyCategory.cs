using UnityEngine;

[ExecuteInEditMode]
public class HierarchyCategory : MonoBehaviour
{
    [SerializeField] private string category = "Category";
    public string Category => category;

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (gameObject.name != category)
            gameObject.name = $"{new string('-', 10)} {category} {new string('-', 10)}";
    }
}