using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils{
    private static AsyncOperation _op = null;

	public static void LoadScene(string name){
		if(name == null){ return; }
		SceneManager.LoadScene(name);
	}
	
	public static string GetCurrentSceneName(){
		return SceneManager.GetActiveScene().name;
	}

	public static void UnPersistObj(GameObject obj){
		SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
	}

    public static void PersistObj(GameObject obj){
        Object.DontDestroyOnLoad(obj);
    }

    public static void PreloadScene(string name){
        if(_op != null){ return; }
        _op = SceneManager.LoadSceneAsync(name);
        _op.allowSceneActivation = false;
    }

	public static void CancelPreload(){
		_op = null;
	}

    public static bool GetPreloadSceneStatus(){
        if(_op == null){ return false; }
        return _op.progress >= 0.9f;
    }

    public static void SwitchToPreloadScene(){
        if(_op == null){ return; }
        if(_op.progress < 0.9f){ return; }
        _op.allowSceneActivation = true;
        _op = null;
    }
}