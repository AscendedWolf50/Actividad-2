using System;

[Serializable]
public class UserData {
    public int id;
    public string name;
    public int[] deck;
}

[Serializable]
public class CharacterData {
    public int id;
    public string name;
    public string occupation;
     public string image;
    public string portrait_path; // URL de la imagen que nos da la API
}