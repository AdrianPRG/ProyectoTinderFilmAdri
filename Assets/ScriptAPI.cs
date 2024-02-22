using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.Networking;
//using static UnityEngine.InputSystem.InputAction;

public class PortraitController : MonoBehaviour
{
    public int numimagen = 0;
    
    // superficie donde mostrar imágenes (hay que asignarla en editor)
    public SpriteRenderer posterSprite;
    private float _current, _target;
    //private NewControls inputSystem;

    // Start is called before the first frame update
    void Start()
    {
        // creamos objeto asociado al input system: NewControls porque se llamaba así el asset de tipo input system
        //inputSystem = new NewControls();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            numimagen = 0;
            StartCoroutine(GetImg());
        }

    }
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator GetImg()
    {
        // Preparo petición datos API (normal)
        UnityWebRequest data = UnityWebRequest.Get("https://www.omdbapi.com/?s=glory&apikey=c12bd3cb");
        // realiza la petición, esperando respuesta
        yield return data.SendWebRequest();

        // se comprueba que no ha fallado conexión
        if (data.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(data.error);
        }
        else
        {
            // ya disponemos de los resultados de la consulta en data.downloadHandler.text
            Debug.Log(data.downloadHandler.text);
            // Pasamos JSON a objetos
            SearchData mySearch = JsonUtility.FromJson<SearchData>(data.downloadHandler.text);
            // Comprobación del resultado
            Debug.Log(mySearch.Search[0].Poster);

            // Preparo petición web de la imagen (Textura)
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(mySearch.Search[numimagen].Poster);
            // realiza la petición, esperando respuesta
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
                    Debug.LogError("Failed to download texture from URL: " + mySearch.Search[numimagen].Poster);
                }
            }
        }
        // fin de la coroutine
        yield break;
    }
}

public class SearchData
{
    public List<MovieData> Search;
    public string totalResults;
    public string Response;
}
[Serializable]
public class MovieData
{
    public string Title;
    public string Year;
    public string imdbID;
    public string Type;
    public string Poster;
}
