using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class movmentS2 : MonoBehaviour
{
    private float timer = 0.0f;
    private int pos = -1;
    private bool moveF = true;
    public float speed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadImage());
    }

    // Update is called once per frame
    void Update()
    {
    }

    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator LoadImage()
    {
        string imageUrl = "https://m.media-amazon.com/images/M/MV5BNDYxNjQyMjAtNTdiOS00NGYwLWFmNTAtNThmYjU5ZGI2YTI1XkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_SX300.jpg";

        // Load the image from the URL
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Failed to load image from URL: " + www.error);
            }
            else
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // Apply the texture to the material of the GameObject
                if (texture != null)
                {
                    Renderer renderer = GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.mainTexture = texture;
                    }
                    else
                    {
                        Debug.LogError("Renderer component not found on GameObject.");
                    }
                }
                else
                {
                    Debug.LogError("Failed to download texture from URL: " + imageUrl);
                }
            }
        }
    }
}