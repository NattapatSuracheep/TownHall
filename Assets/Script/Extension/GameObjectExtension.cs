using UnityEngine;

public static class GameObjectExtension
{
    public static void Active(this GameObject gameObject)
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
    }

    public static void DeActive(this GameObject gameObject)
    {
        if (gameObject.activeSelf == true)
            gameObject.SetActive(false);
    }

    public static void Active(this Transform gameObject) => gameObject.transform.gameObject.Active();
    public static void DeActive(this Transform gameObject) => gameObject.transform.gameObject.DeActive();
}