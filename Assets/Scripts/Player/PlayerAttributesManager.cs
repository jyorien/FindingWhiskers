using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerAttributesManager manages the player's attributes decreasing over time.
/// The attributes decrease logarithmically over a set time that can be defined in the inspector.
/// It is also responsible for updating the current value of the attributes in PlayerAttributesDataSO for other classes to access.
/// </summary>
public class PlayerAttributesManager : MonoBehaviour
{
    // store in variable so we can control start/stopping the coroutine
    private IEnumerator decreaseValueAttributesOverTime;

    [Header("Time to Reach Minimum")]
    [SerializeField] private int timeTakenToReachMinimum;

    [Header("Scriptable Objects")]
    [SerializeField] private PlayerAttributesDataSO playerAttributes;

    // store max and min attributes
    // player will always start off with max values and gradually decrease to min

    [Header("Jump")]
    [SerializeField] private float maxJumpForce;
    [SerializeField] private float minJumpForce;

    [Header("Movement Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;

    [Header("Size Scale")]
    [SerializeField] private float maxSizeScale = 1.3f;
    [SerializeField] private float minSizeScale = 0.6f;


    // Start is called before the first frame update
    void Start()
    {
        // start the coroutine for the first time when a level loads
        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);
        ResetToMaxAttributeValues();
    }

    public void ResetToMaxAttributeValues()
    {
        // reset the coroutine
        StopCoroutine(decreaseValueAttributesOverTime);
        decreaseValueAttributesOverTime = DecreaseAttributeValuesOverTime(timeTakenToReachMinimum);

        // when starting the game or touching a campfire, reset jump force, speed and size
        playerAttributes.ChangeAttributes(maxSpeed, maxJumpForce);
        transform.localScale = new Vector3(maxSizeScale, maxSizeScale, 1);

        // restart the coroutine
        StartCoroutine(decreaseValueAttributesOverTime);
    }

    private IEnumerator DecreaseAttributeValuesOverTime(int durationInSeconds)
    {
        float timePassed = 0f;
        Vector3 maxScaleVector3 = new Vector3(maxSizeScale, maxSizeScale, 1);
        Vector3 minScaleVector3 = new Vector3(minSizeScale, minSizeScale, 1);

        while (timePassed <= durationInSeconds)
        {
            timePassed += Time.deltaTime;

            /* scale the attributes (speed, jump force, and size) logarithmically based on how much the time has passed compared to how long its supposed
             * to take to reach the minimum value of the attribute
             *
             * scaling it logarithmically makes it decrease faster at the start 
             * and decrease slower towards the end
             * 
             * this simulates the experience of the player rapidly starting to feel "cold" at the start
             * but the difference in "feeling cold" is smaller towards the end
             */
            float t = Mathf.Log10(1 + timePassed) / Mathf.Log10(1 + durationInSeconds);

            float currentSpeed = Mathf.Lerp(maxSpeed, minSpeed, t);
            float currentJumpForce = Mathf.Lerp(maxJumpForce, minJumpForce, t);

            playerAttributes.ChangeAttributes(currentSpeed, currentJumpForce);
            transform.localScale = Vector3.Lerp(maxScaleVector3, minScaleVector3, t);

            yield return null;
        }
    }
}
