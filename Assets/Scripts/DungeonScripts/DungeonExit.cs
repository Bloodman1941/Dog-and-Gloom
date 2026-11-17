using UnityEngine;
using System.Collections;

public class DungeonExit : MonoBehaviour
{
    private RoomFirstDungeonGenerator dungeonGenerator;
    private static bool isTransitioning = false;

    public void Initialized(RoomFirstDungeonGenerator generator)
    {
        dungeonGenerator = generator;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTransitioning) return;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(TransitionToNextLevel());
        }
    }

    private IEnumerator TransitionToNextLevel()
    {
        isTransitioning = true;
        
        CanvasGroup fadeGroup = FindAnyObjectByType<CanvasGroup>();
        float fadeDuration = 0.35f;

        if (fadeGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                yield return null;
            }
        }

        if (dungeonGenerator != null)
        {
            dungeonGenerator.GenerateDungeon();
        }

        if (fadeGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeGroup.alpha = Mathf.Clamp01(1 - (elapsedTime / fadeDuration));
                yield return null;
            }
        }

        isTransitioning = false;
    }
}
