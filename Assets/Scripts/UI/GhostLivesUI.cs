using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLivesUI : MonoBehaviour
{
    [SerializeField] private GameObject[] ghostLives;

    // Update is called once per frame
    private void Update()
    {
        // get how many hearts to hide
        int ghostLivesUsed = GhostEnemy.hitCount;

        // in case lives go over 3, set it to 3 to avoid making index out of bounds
        if (ghostLivesUsed > 3) ghostLivesUsed = 3;

        // loop thrice as Ghost has 3 lives
        for (int i = 0; i < 3; i++)
        {
            // hide hearts based on hit count
            if (i < ghostLivesUsed)
            {
                ghostLives[i].SetActive(false);

            }
        }
    }
}
