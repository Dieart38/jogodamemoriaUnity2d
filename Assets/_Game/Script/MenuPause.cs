using UnityEngine;
using System.Collections; // Necessário para usar Coroutines (IEnumerator)
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class MenuPause : MonoBehaviour
{
    [Header("Referências Principais")]    
    public GameObject painelDePause;
    public GameObject telaAgradecimento;

    [Header("Sub-Menus")]
    public GameObject painelDeConfiguracoes;
    public GameObject menuPrincipalPause; // Referência ao menu principal do pause, para poder reativá-lo quando fechar o painel de configurações
    private bool jogoPausado = false;

    void Start()
    {
        // Garante que o jogo começa despausado e com os painéis fechados
        if (painelDePause != null)
        {
            painelDePause.SetActive(false);
        }
        if (telaAgradecimento != null)
        {
            telaAgradecimento.SetActive(false);
        }
        if (painelDeConfiguracoes != null)
        {
            painelDeConfiguracoes.SetActive(false);
        }
        
        // BLINDAGEM: Garante que a fase começa 100% descongelada e com som!
        Time.timeScale = 1f;
        AudioListener.pause = false; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Escape) ||
            Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            AlternarPause();
        }
    }

    public void AlternarPause()
    {
        jogoPausado = !jogoPausado;

        if (jogoPausado)
        {
            Pausar();
        }
        else
        {
            Continuar();
        }
    }

    public void Pausar()
    {
        jogoPausado = true;
        if (painelDePause != null)
        {
            painelDePause.SetActive(true);
        }
        // Quando pausar, garante que sempre mostre os botões primeiro e esconda as opções
        if (menuPrincipalPause != null) menuPrincipalPause.SetActive(true);
        if (painelDeConfiguracoes != null) painelDeConfiguracoes.SetActive(false);
        
        Time.timeScale = 0f; // Congela o tempo do jogo
        AudioListener.pause = true; // PAUSA TODOS OS SONS!
    }

    public void Continuar()
    {
        jogoPausado = false;
        if (painelDePause != null)
        {
            painelDePause.SetActive(false);
        }
        
        Time.timeScale = 1f; // Retoma o tempo normal do jogo
        AudioListener.pause = false; // RETOMA OS SONS DE ONDE PARARAM!
    }

    public void openConfigPanel()
    {
        if (painelDeConfiguracoes != null)
        {
            menuPrincipalPause.SetActive(false); // Esconde o menu principal do pause
            painelDeConfiguracoes.SetActive(true);
        }
    }
    
    public void closeConfigPanel()
    {
        if (painelDeConfiguracoes != null)
        {
            painelDeConfiguracoes.SetActive(false);
            menuPrincipalPause.SetActive(true); // Mostra o menu principal do pause novamente
        }
    }

    public void FecharMenuPause()
    {
        Continuar(); // Centralizado: chama a mesma função de Continuar para não duplicar código
    }

    // voltar ao menu iniciar
    public void VoltarAoMenuIniciar()
    {
        // Desativa o menu de pause para não ficar sobreposto
        if (painelDePause != null)
        {
            painelDePause.SetActive(false);
        }

        // BLINDAGEM: Se o jogador voltar ao menu, destrave o tempo e o áudio antes de mudar de cena!
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // volta para o menu iniciar
        SceneManager.LoadScene("MenuIniciar");
    }

    public void SairDoJogo()
    {
        // Desativa o menu de pause para não ficar sobreposto
        if (painelDePause != null)
        {
            painelDePause.SetActive(false);
        }

        // Mostra a tela de agradecimento (ex: "Obrigado por jogar, Diego Soares!")
        if (telaAgradecimento != null)
        {
            telaAgradecimento.SetActive(true);
        }

        // Inicia a contagem de tempo real
        StartCoroutine(AguardarEFechar());
    }

    private IEnumerator AguardarEFechar()
    {
        // Usa o tempo REAL, ignorando o fato de que Time.timeScale é 0
        yield return new WaitForSecondsRealtime(1.5f);

        Application.Quit();

        // Permite testar o fechamento dentro do próprio Editor da Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    
    public void ReiniciarDoPause()
    {
        // 1. Primeiro, nós saímos do estado de pause formalmente (o Continuar já despausa o áudio)
        Continuar(); 
        
        // 2. Agora sim, mandamos o jogo reiniciar
        UIManager.Instance.BotaReiniciar(); 
    }
}