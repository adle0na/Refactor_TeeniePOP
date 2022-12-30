//Copyright 2013-2022 AFI, INC. All right reserved

using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticManager : MonoBehaviour {
    public static StaticManager Instance { get; private set; }

    public static BackendManager Backend { get; private set; }
    public static UIManager UI { get; private set; }

    
    // 모든 씬에서 사용되는 기능들을 모아놓은 클래스.
    // 각씬메니저가 현재 쌘에 존재하는지 확인 후 생성한다.
    void Awake() {
        Init();
    }

    void Init() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Backend = GetComponentInChildren<BackendManager>();
        UI      = GetComponentInChildren<UIManager>();

        UI.Init();
        Backend.Init();

        
    }

    // 씬 변경 시 페이드아웃되면서 씬 전환
    public void ChangeScene(string sceneName) {
        UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToBlack, () => SceneManager.LoadScene(sceneName));
    }
}