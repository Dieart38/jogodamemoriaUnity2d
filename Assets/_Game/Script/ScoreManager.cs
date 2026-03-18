using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerScore {
    public string nome;
    public int tempo;
    public int tentativas;
}

public static class ScoreManager {
    private const int MaxRecordes = 6;

    public static void SalvarRecorde(string nome, int tempo, int tentativas) {
        List<PlayerScore> lista = CarregarRecordes();
        lista.Add(new PlayerScore { nome = nome, tempo = tempo, tentativas = tentativas });

        // Ordena por menor tempo (ou tentativas se preferir)
        lista.Sort((a, b) => a.tempo.CompareTo(b.tempo));

        // Mantém apenas os 6 melhores
        if (lista.Count > MaxRecordes) lista.RemoveAt(MaxRecordes);

        // Converte para JSON e salva no PlayerPrefs
        string json = JsonUtility.ToJson(new SerializationWrapper<PlayerScore> { items = lista });
        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save();
    }

    public static List<PlayerScore> CarregarRecordes() {
        if (!PlayerPrefs.HasKey("HighScores")) return new List<PlayerScore>();
        string json = PlayerPrefs.GetString("HighScores");
        return JsonUtility.FromJson<SerializationWrapper<PlayerScore>>(json).items;
    }

    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }
}