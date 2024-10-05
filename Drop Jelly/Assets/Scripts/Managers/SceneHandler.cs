using ww.Utilities.Singleton;
using UnityEngine.SceneManagement;
namespace ww.DropJelly
{
    public class SceneHandler : Singleton<SceneHandler>
    {
        public void LoadNextLevel()
        {
            SceneManager.LoadScene(0);
        }
        public void ReloadCurrentLevel()
        {
            SceneManager.LoadScene(0);
        }
    }
}
