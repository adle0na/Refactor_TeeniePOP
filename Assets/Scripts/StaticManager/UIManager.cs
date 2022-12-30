//Copyright 2013-2022 AFI,INC. All right reserved.

using System.Reflection;
using InGameScene;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager : MonoBehaviour {
    [SerializeField]
    private FadeUI _fadeUI;
    [SerializeField]
    private AlertUI _alertUI;

    // 알림 UI
    public AlertUI AlertUI {
        get { return _alertUI; }
    }

    //페이드 in/out UI
    public FadeUI FadeUI {
        get { return _fadeUI; }
    }

    [SerializeField] private GameObject _loadingAnimationIcon;


    public void Init() {
        AlertUI.Init();
        FadeUI.Init();
        
        AlertUI.gameObject.SetActive(false);
        FadeUI.gameObject.SetActive(false);
        _loadingAnimationIcon.gameObject.SetActive(false);

    }

    // ====================================================================
    // 오브젝트가 없을 경우, Resources에서 검색하여 생성한다.
    // ====================================================================
    private bool TryLoadUIObject(string prefabName, Transform parent, out GameObject gameObject) {
        gameObject = null;

        string path = $"{prefabName}";
        GameObject loadObject = Resources.Load<GameObject>(path);

        if (loadObject == null) {
            Debug.LogError($"{prefabName}가 Prefab에 존재하지 않습니다. in {path}");
            return false;
        }

        gameObject = Object.Instantiate(loadObject, parent, true);
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.localPosition = Vector3.zero;

        return true;
    }

    // ====================================================================
    // 로딩 아이콘은 on/off한다.
    // ====================================================================
    public void SetLoadingIcon(bool active) {
        _loadingAnimationIcon.SetActive(active);
    }

    // ====================================================================
    // 현재 씬에 UICanvas를 생성한다. 씬이 이동할 경우 없어지며 새로운 UI를 만든다.
    // 생성만 가능하기에 삭제가 가능한 Resources의 오브젝트만 해당 UI로 할당한다.
    // ====================================================================
    public void OpenUI<T>(string folderPath, Transform parent) where T : BaseUI{
        if (TryLoadUIObject(folderPath + "/"+ typeof(T).Name, parent, out var uiObject) == false) {
            
            StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name,MethodBase.GetCurrentMethod()?.ToString(), folderPath + "를 찾을 수 없습니다.");
            return;
        }
        
        uiObject.GetComponent<BaseUI>().OpenUI();
    }

}