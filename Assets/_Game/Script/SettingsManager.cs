using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    // Padrão Singleton para acesso global
    public static SettingsManager Instance { get; private set; }

    [Header("Acessibilidade")]
    public UnityEngine.UI.Toggle toggleTremor; // Arraste o Toggle da UI aqui

    [Header("Referências")]
    public AudioMixer mainMixer;
    
    [Tooltip("Os Sliders devem ir de 0.0001 (mínimo) a 1 (máximo)")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider cursorSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: Se quiser que as configs sobrevivam entre as cenas
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 1. CARREGAR DADOS SALVOS (PlayerPrefs)
        // Se não tiver nada salvo, usa um valor padrão (ex: 0.75 para áudio, 500 para cursor)
        float savedMusic = PlayerPrefs.GetFloat("MusicVol", 0.75f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVol", 0.75f);
        float savedCursor = PlayerPrefs.GetFloat("CursorSpeed", 500f);
        // Carrega o estado salvo do tremor (1 = ligado, 0 = desligado)
        if (toggleTremor != null)
        {
            // Marca a caixinha como checada se o valor for 1
            toggleTremor.isOn = PlayerPrefs.GetInt("TremerTela", 1) == 1;
            
            // Cria um "ouvinte" que avisa o sistema sempre que o jogador clicar na caixinha
            toggleTremor.onValueChanged.AddListener(DefinirTremor);
        }

        // 2. ATUALIZAR A INTERFACE (Mover os sliders para a posição correta)
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;
        if (cursorSlider != null) cursorSlider.value = savedCursor;

        // 3. ADICIONAR OUVINTES (Listeners)
        // Isso dispensa a necessidade de você ir no Inspector arrastar eventos "OnValueChanged"
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        if (cursorSlider != null) cursorSlider.onValueChanged.AddListener(SetCursorSpeed);

        // 4. APLICAR OS VOLUMES NO INÍCIO DO JOGO
        ApplyVolume("MusicVol", savedMusic);
        ApplyVolume("SFXVol", savedSFX);
    }

    // --- MÉTODOS PÚBLICOS PARA OS SLIDERS ---

    public void SetMusicVolume(float sliderValue)
    {
        ApplyVolume("MusicVol", sliderValue);
        PlayerPrefs.SetFloat("MusicVol", sliderValue); // Salva a preferência
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        ApplyVolume("SFXVol", sliderValue);
        PlayerPrefs.SetFloat("SFXVol", sliderValue); // Salva a preferência
        PlayerPrefs.Save();
    }

    private void ApplyVolume(string exposedParam, float sliderValue)
    {
        // A MATEMÁTICA PROFISSIONAL (Log10)
        // Sliders geralmente vão de 0 a 1. O Log10 de 0 é menos infinito, o que quebra o áudio.
        // Por isso clampamos o mínimo em 0.0001f.
        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        
        // Fórmula de conversão Linear para Decibel (dB)
        float decibels = Mathf.Log10(clampedValue) * 20f;
        
        mainMixer.SetFloat(exposedParam, decibels);
    }

    public void SetCursorSpeed(float sliderValue)
    {
        PlayerPrefs.SetFloat("CursorSpeed", sliderValue);
        PlayerPrefs.Save();
        
        // Busca o cursor na tela e atualiza em tempo real sem precisar reiniciar
        ControllerCursor cursor = FindFirstObjectByType<ControllerCursor>();
        if (cursor != null)
        {
            cursor.velocidade = sliderValue;
        }
    }
    // Método que é chamado automaticamente quando o jogador clica no Toggle
    public void DefinirTremor(bool ativado)
    {
        // Se ativado for true, salva 1. Se false, salva 0.
        PlayerPrefs.SetInt("TremerTela", ativado ? 1 : 0);
        PlayerPrefs.Save(); // Grava no celular/PC imediatamente
    }
}