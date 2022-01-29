using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIHandler : MonoBehaviour
{
    public TextMeshProUGUI playerName;

    public void StartGame()
    {
        SetName();
        SceneManager.LoadScene(1);
    }

    private void SetName()
    {
        if (PersistenceManager.Instance != null)
        {
            PersistenceManager.Instance.PlayerName = playerName.text;
        }
    }
}
