using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void SelectEasyLevel()
    {
        PlayButtonSound();
        Debug.Log("Memulai Level: EASY (Draf Bab 1-3)");
        SceneManager.LoadScene("Easy");
    }

    public void SelectMediumLevel()
    {
        PlayButtonSound();
        Debug.Log("Memulai Level: MEDIUM (Dosen Pembimbing)");
        SceneManager.LoadScene("Medium");
    }

    public void SelectHardLevel()
    {
        PlayButtonSound();
        Debug.Log("Memulai Level: HARD (Sidang Akhir)");
        SceneManager.LoadScene("Hard");
    }

    public void ExitGame()
    {
        PlayButtonSound();
        Debug.Log("Keluar dari Game... (Fungsi aktif setelah di-build)");
        Application.Quit();
    }

    public void PlayButtonSound()
    {
        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.PlaySFX(AudioManagers.Instance.buttonClickSFX);
        }
    }
}