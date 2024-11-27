using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    // Cambiar a una escena específica
    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    // Cargar la escena de Inicio
    public void LoadStartScene() {
        SceneManager.LoadScene("Inicio");
    }

    // Cargar la escena de Animación
    public void LoadAnimationScene() {
        SceneManager.LoadScene("Animacion");
    }

    // Cargar la escena de Game Over
    public void LoadGameOverScene() {
        SceneManager.LoadScene("JuegoPerdido");
    }

    // Cargar la escena de Juego Ganado
    public void LoadWinScene() {
        SceneManager.LoadScene("JuegoGanado");
    }

    // Salir del juego
    public void QuitGame() {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
