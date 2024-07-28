using UnityEngine;
using CrazyMinnow.SALSA;

public class SoundStrength : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private float minDb = 0f; // Define the minimum dB level you are interested in
    private float maxDb = 120f; // Define the maximum dB level you are interested in
    [SerializeField] private Salsa salsa;



    private void Update()
    {
        float currentDb = GetDecibelLevel();
        float soundStrength = MapDbToStrength(currentDb);
        salsa.analysisValue = soundStrength / 100;
        if (soundStrength/100 != 0)
        {
            // Debug.Log(soundStrength / 100);
        }
    }

    private float GetDecibelLevel()
    {
        float rmsValue = 0.0f;
        float dbValue = 0.0f;
        float[] samples = new float[1024];
        audioSource.GetOutputData(samples, 0);

        foreach (float sample in samples)
        {
            rmsValue += Mathf.Pow(sample, 2);
        }

        rmsValue = Mathf.Sqrt(rmsValue / 1024);
        dbValue = 20 * Mathf.Log10(rmsValue / 0.0001f);

        return dbValue;
    }


    private float MapDbToStrength(float db)
    {
        return Mathf.InverseLerp(minDb, maxDb, db) * 100;
    }
}
