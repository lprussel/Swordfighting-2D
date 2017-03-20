using UnityEngine;
using System.Collections;

public class Screenshake : MonoBehaviour
{
    public static Screenshake instance;
    private Vector3 initialPos;

    void Awake ()
    {
        instance = this;
    }

    void Start ()
    {
        initialPos = transform.localPosition;
    }

    public void Shake(float shakeMag, float shakeTime)
    {
        StartCoroutine(_Shake(shakeMag, shakeTime));
    }

    IEnumerator _Shake(float shakeMag, float shakeTime)
    {
        float t = 0;
        while (t < shakeTime)
        {
            t += Time.deltaTime;
            Vector3 rand = new Vector3(Random.Range(-shakeMag, shakeMag) * (1 - (t / shakeTime)), Random.Range(-shakeMag, shakeMag) * (1 - (t / shakeTime)), 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos + rand, Time.deltaTime * 5);
            yield return null;
        }

        transform.localPosition = initialPos;
    }
}
