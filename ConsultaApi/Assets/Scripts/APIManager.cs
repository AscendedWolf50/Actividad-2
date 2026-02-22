using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class APIManager : MonoBehaviour {
    [Header("Configuración de URLs")]
    public string myJsonServerUrl = "https://my-json-server.typicode.com/AscendedWolf50/Actividad-2/users/";
    private string simpsonsApiUrl = "https://thesimpsonsapi.com/api/characters/";

    [Header("Referencias de UI")]
    public TextMeshProUGUI authorNameText; 
    public TextMeshProUGUI currentPlayerText; 
    public Transform deckContainer; 
    public GameObject cardPrefab; 

    private int currentUserId = 1;

    void Start() {
        authorNameText.text = "Desarrollado por: Juan José Builes";
        LoadData();
    }

    public void SwitchUser() {
        currentUserId = (currentUserId == 1) ? 2 : 1;
        LoadData();
    }

    void LoadData() {
        foreach (Transform child in deckContainer) Destroy(child.gameObject);
        StartCoroutine(GetUserData());
    }

    IEnumerator GetUserData() {
        using (UnityWebRequest www = UnityWebRequest.Get(myJsonServerUrl + currentUserId)) {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                UserData user = JsonUtility.FromJson<UserData>(www.downloadHandler.text);
                currentPlayerText.text = "Jugador Actual: " + user.name;

                foreach (int cardId in user.deck) {
                    StartCoroutine(GetSimpsonsCharacter(cardId));
                }
            }
        }
    }

    IEnumerator GetSimpsonsCharacter(int id) {
    // La URL de la API es correcta según los docs
    string requestUrl = "https://thesimpsonsapi.com/api/characters/" + id;
    
    using (UnityWebRequest www = UnityWebRequest.Get(requestUrl)) {
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            CharacterData character = JsonUtility.FromJson<CharacterData>(www.downloadHandler.text);
            
            GameObject newCard = Instantiate(cardPrefab, deckContainer);
            newCard.GetComponentInChildren<TextMeshProUGUI>().text = character.name.ToUpper();

            // AJUSTE SEGÚN DOCUMENTACIÓN:
            // Usamos el CDN específico para las imágenes
            // Usamos un servicio gratuito (wsrv.nl) que recibe el .webp y lo entrega como .png
string rawImageUrl = "https://cdn.thesimpsonsapi.com/500" + character.portrait_path;
string imageUrl = "https://wsrv.nl/?url=" + rawImageUrl + "&output=png";

Debug.Log("Cargando imagen convertida: " + imageUrl);
StartCoroutine(DownloadCardImage(imageUrl, newCard.GetComponentInChildren<RawImage>()));
        }
    }
}

    IEnumerator DownloadCardImage(string url, RawImage img) {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url)) {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success) {
                img.texture = DownloadHandlerTexture.GetContent(www);
            } else {
                // Si esto sale, es que la URL que armamos sigue siendo incorrecta o el formato no es compatible
                Debug.LogWarning("Error al descargar: " + url + " | " + www.error);
            }
        }
    }
}