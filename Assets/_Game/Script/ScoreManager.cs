using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PlayerScore
{
    public string nome;
    public int tempo;
    public int tentativas;
    public int fasesJogadas; // Novo campo
}
public static class ScoreManager
{
    private const int MaxRecordes = 6;

    public static void SalvarRecorde(string nome, int tempo, int tentativas, int fases)
    {
        List<PlayerScore> lista = CarregarRecordes();
        lista.Add(new PlayerScore
        {
            nome = nome,
            tempo = tempo,
            tentativas = tentativas,
            fasesJogadas = fases
        });

        lista.Sort((a, b) =>
        {
            int res = b.fasesJogadas.CompareTo(a.fasesJogadas);
            if (res == 0) res = a.tempo.CompareTo(b.tempo);
            return res;
        });

        if (lista.Count > MaxRecordes) lista.RemoveAt(MaxRecordes);

        // --- ADICIONE ESTAS LINHAS ABAIXO PARA GRAVAR DE VERDADE ---
        string json = JsonUtility.ToJson(new SerializationWrapper<PlayerScore> { items = lista });
        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save(); // Força a gravação no arquivo
    }

    public static List<PlayerScore> CarregarRecordes()
    {
        if (!PlayerPrefs.HasKey("HighScores")) return new List<PlayerScore>();
        string json = PlayerPrefs.GetString("HighScores");
        return JsonUtility.FromJson<SerializationWrapper<PlayerScore>>(json).items;
    }

    [Serializable]
    private class SerializationWrapper<T> { public List<T> items; }
}