using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ExitButton : MonoBehaviour {

    public ExitType exitType = ExitType.Exit;
    
    public enum ExitType {
        Exit,
        ActiveFalse
    }
    void Start() {
        if (exitType == ExitType.Exit) {
            GetComponentInChildren<Button>().onClick.AddListener(CloseUI);

        }
        else if (exitType == ExitType.ActiveFalse) {
            GetComponentInChildren<Button>().onClick.AddListener(SetActivefalseUI);
        }
    }

    void CloseUI() {
        Destroy(GetComponentInParent<BaseUI>().gameObject);
    }
    void SetActivefalseUI() {
        transform.parent.parent.gameObject.SetActive(false);
    }
}
