using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.Serialization;

//using static UnityEngine.InputSystem.InputAction;

public class PortraitController : MonoBehaviour,Teclado.ITeclaAActions,Teclado.ITeclaDActions
{
    
    public int numimagen = 0;
    int numpagina = 1;
    private int contimage = 0;
    private Teclado.TeclaAActions _teclaA;
    private Teclado.TeclaDActions _teclaD;
    public TextMeshPro titulo;
    public TextMeshPro año;
    
    // superficie donde mostrar imágenes (hay que asignarla en editor)
    public SpriteRenderer posterSprite;
    private float _current, _target;
    //private NewControls inputSystem;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetImg("https://www.omdbapi.com/?s=glory&apikey=c12bd3cb&page=1"));
        _teclaA = new Teclado().TeclaA;
        _teclaD = new Teclado().TeclaD;
        _teclaA.Enable();
        _teclaD.Enable();
        _teclaA.AddCallbacks(this);
        _teclaD.AddCallbacks(this);
        
    }

    private void OnDisable()
    {
        _teclaA.Disable();
        _teclaD.Disable();
    }
    
    
    
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator GetImg(string url)
    {
        // Preparo petición datos API (normal)
        UnityWebRequest data = UnityWebRequest.Get(url);
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
                        titulo.text = mySearch.Search[numimagen].Title;
                        año.text =  mySearch.Search[numimagen].Year;
                        renderer.material.mainTexture = texture;
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

    public void OnOnTeclaA(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            string url = "https://www.omdbapi.com/?s=glory&apikey=c12bd3cb&page="+numpagina;
            
            if (contimage < 9)
            {
                numimagen++;
                StartCoroutine(GetImg(url));
                contimage++;
            }
            else
            {
                contimage = 0;
                numimagen = 0;
                StartCoroutine(GetImg(url));

            }
        }
    }

    public void OnOnTeclaD(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            numpagina++;
            string url = "https://www.omdbapi.com/?s=glory&apikey=c12bd3cb&page="+numpagina;
            StartCoroutine(GetImg(url));
        }
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
