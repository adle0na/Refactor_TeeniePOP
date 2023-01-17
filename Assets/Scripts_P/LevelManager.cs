using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using InGameScene;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // 싱글톤 사용
    public static LevelManager Instance { get; private set; }
    
    #region Serialize변수

    [SerializeField] private LevelData levelDB;
    
    // 티니핑 프리팹 배열 ( 현재 샘플로 과일 사용중 )
    [SerializeField]
    private GameObject[]    TPingPrefabs;
    // 라인 렌더러
    [SerializeField]
    private LineRenderer    LineRenderer;
    // 폭탄 프리팹
    [SerializeField]
    private GameObject      BombPrefab;
    // 리미트 초기화 값
    [SerializeField]
    private TextMeshProUGUI DragPointText;
    
    // 인게임 팝업
    [Header("인게임 팝업")]
    [SerializeField]
    private GameObject[]    InGamePopUps;
    // 0: 리마인드, 1: 게임오버, 2: 클리어, 3: Pause메뉴 4: 종료확인
    
    // 인게임 팝업 배경
    [SerializeField]
    private GameObject      InGamePopUpBG;
    
    [Header("목표(상단바)")]
    [SerializeField]
    private GameObject[] Targets_Top;
    
    [Header("목표(리마인드)")]
    [SerializeField]
    private GameObject[] Targets_Remind;
    
    [Header("목표 값")]
    [SerializeField]
    private int[] targetIndex = new int[6];
    
    #endregion
    
    #region 숨김 변수 (public)
    
    // 전체 티니핑 관리 배열
    [Header("전체 블록")]
    public List<Tping> _allTpings   = new List<Tping>();
    // 티니핑 선택 배열
    [Header("선택된 블록")]
    public List<Tping> _selectPings = new List<Tping>();

    [Header("파괴되는 블록")]
    public List<Tping> _erasePings  = new List<Tping>();
    
    // 선택된 티니핑 ID
    [HideInInspector]
    public int          _selectedID;
    // 플레이 체크
    [HideInInspector]
    public bool   _isPlaying;
    // 제한 횟수
    [HideInInspector]
    public int DragPoint;
    
    // 아이템 2 상태 체크
    [HideInInspector]
    public bool PopUsingCheck;
    
    #endregion

    #region Private 변수
    
    // 타겟 정보
    private Target _target;
    // 체인 블록
    private Bomb   _bomb;
    // 승리 체크
    private bool   _isClear = false;
    // 점수 값
    private int    _Score = 0;

    private int    level;
    
    private LevelSort _sort;
    
    // 최소 연결 횟수
    private int          TpingDestroyCount = 3;
    // 연결 범위
    private float        TpingConnectRange = 1.1f;
    // 체인블록 최소 생성 카운트
    private int          BombSpawnCount    = 5;
    // 폭발 범위
    private float        BombDestroyRange  = 1.5f;
    // 연결 사용 감소 횟수
    private int          CalDragPoint      = 1;

    #endregion
    
    // 나중에 스테이지 시작 함수로 변경할것
    void Awake()
    {
        level = PlayerPrefs.GetInt("SelectedLevel");
        Instance = this;

        _isPlaying = true;
        PopUsingCheck = false;

        DragPoint             = levelDB.Sheet1[level].drag;
        DragPointText.text    = DragPoint.ToString();
        string[] targets      = levelDB.Sheet1[level].targets.Split(", ");
        string[] targetValues = levelDB.Sheet1[level].targetValue.Split(", ");
        
        for (int i = 0; i < targets.Length; i++)
        {
            int result;
            int.TryParse(targets[i], out result);

            int result2;
            int.TryParse(targetValues[i], out result2);
            
            targetIndex[result] = result2;
            
            int id = Targets_Top[result].GetComponent<Target>().Target_ID;
            
            Targets_Top[result].SetActive(true);
            Targets_Top[result].GetComponentInChildren<TextMeshProUGUI>().text = targetIndex[id].ToString();

            int id2 = Targets_Remind[result].GetComponent<Target>().Target_ID;
            
            Targets_Remind[result].SetActive(true);
            Targets_Remind[result].GetComponentInChildren<TextMeshProUGUI>().text = targetIndex[id2].ToString();
        }

        StartCoroutine(TargetRemind());
    }

    #region 미사용 함수

    void Update()
    {
        //LineRendererUpdate();
    }
    
    // 라인 렌더러 설정
    private void LineRendererUpdate()
    {
        // if (_selectPings.Count >= 2)
        // {
        //     LineRenderer.positionCount = _selectPings.Count;
        //
        //     LineRenderer.SetPositions(_selectPings.Select(tping => tping.transform.position).ToArray());
        //     
        //     LineRenderer.gameObject.SetActive(true);
        // }
        // else LineRenderer.gameObject.SetActive(false);
    }

    #endregion
    
    // 생성 관리 함수
    private void TPingSpawn(int count)
    {
        List<GameObject> spawnList = new List<GameObject>();
        
        var StartX = -2;
        var StartY = 5;
        var X = 0;
        var Y = 0;
        var MaxX = 5;
        
        string[] spawnPings = levelDB.Sheet1[level].spawnPing.Split(", ");

        for (int i = 0; i < spawnPings.Length; i++)
        {
            int result;
            int.TryParse(spawnPings[i], out result);
            
            spawnList.Add(TPingPrefabs[result]);
        }

        for (int i = 0; i < count; i++)
        {
            var Position = new Vector3(StartX + X , StartY + Y, 0);
            var TpingObejct =
                Instantiate(spawnList[Random.Range(0, spawnList.Count)], Position, Quaternion.identity);
            
            _allTpings.Add(TpingObejct.GetComponent<Tping>());
            
            X ++;
            if (X == MaxX)
            {
                X = 0;
                Y++;
            }
        }
    }
    
    // 체인 블록 생성
    public void MakeBomb(int num, float size)
    {
        BombPrefab.transform.localScale =
            new Vector3(transform.localScale.x * size,
                transform.localScale.y * size,
                transform.localScale.z * size);
        Instantiate(BombPrefab, _selectPings[0].transform.position, Quaternion.identity);
        Bomb.BInstance._bombAnim.SetInteger("Bomb_num", num);
        Bomb.BInstance.bomb_num = num;
    }
    
    #region 마우스 인풋 함수
    
    // 클릭하여 선택 시작
    public void PingDown(Tping tping)
    {
        // 예외처리
        if (!_isPlaying) return;

        if (!_selectPings.Contains(tping))
        {
            _selectedID = tping.ID;
            _selectPings.Add(tping);
            tping.SetIsSelect(true);
        }
    }
    
    // 드래그
    public void PingEnter(Tping tping)
    {
        // // 예외처리
        // if (!_isPlaying) return;
        // if (_selectID != tping.ID) return;
        //
        // // 티니핑 선택시
        // if (tping.IsSelect)
        // {
        //     if (_selectPings.Count >= 2 && _selectPings[_selectPings.Count - 2] == tping)
        //     {
        //         var RemoveTping = _selectPings[_selectPings.Count - 1];
        //         RemoveTping.SetIsSelect(false);
        //         _selectPings.Remove(RemoveTping);
        //     }
        // }
        // // 주변 선택시
        // else
        // {
        //     var length = (_selectPings[_selectPings.Count - 1].transform.position
        //                   - tping.transform.position).magnitude;
        //     if (length < TpingConnectRange)
        //     {
        //         _selectPings.Add(tping);
        //         tping.SetIsSelect(true);
        //     }
        // }
    }
    
    // 선택 종료
    public void PingUp()
    {
        if (!_isPlaying) return;

        int selectN = _selectPings.Count;
        
        if (selectN >= TpingDestroyCount)
        {
            foreach (var tpingItem in _selectPings)
            {
                tpingItem.isBoom = true;
            }
            
            for (int i = 3; i > 0; i--) 
            {
                if (selectN >= BombSpawnCount + (i * 2) - 2)
                {
                    MakeBomb(i, (i * 0.01f));
                    break;
                }
            }
            SubsDragPoint();
            // 그 이하는 체인블록 미생성, 연결 블록만 제거
            TargetClear(_selectPings);
            DestroyTpings(_selectPings);
        }       
        else
        {
            foreach (var tpingItem in _selectPings)
            {
                tpingItem.isBoom = false;
                tpingItem.SetIsSelect(false);
            }
            _selectPings.Clear();
        }
    }
    
    public void PopUse()
    {
        if (PopUsingCheck = true) return;
        
        PopUsingCheck = true;
    }
    
    // 체인블록 사용
    public void BombDown(Tping tping) 
    {
        // 플레이 체크
        if (!_isPlaying) return;

        if (PopUsingCheck)
        {
            foreach (var TpingItem in _allTpings)
            {
                var Length = (TpingItem.transform.position - tping.transform.position).magnitude;
                // 체인블록 상태 검증 [ 크기, 특수블록 레벨 ]
            
                if (Length < (BombDestroyRange * 1.5f))
                    _erasePings.Add(TpingItem);
                break;
            }
        
            TargetClear(_erasePings);
            DestroyTpings(_erasePings);
            PopUsingCheck = false;
        }

    }
    
    // 블록 파괴시 실행 (점수 증가, 블록 재생성 관리 )
    public void DestroyTpings(List<Tping> tpings)
    {
        StartCoroutine(DestroyEffect(tpings));
        TPingSpawn(tpings.Count);
        AddScore(tpings.Count);
    }

    IEnumerator DestroyEffect(List<Tping> tpings)
    {
        if (tpings == null)
            yield break;
        
        for (int i = 0; i < tpings.Count; i++)
        {
            tpings[i].SelectSprite.SetActive(false);
            tpings[i].clearEffect.SetActive(true);
            tpings[i].clearAnim.SetBool("Cleared", true);
        }

        yield return new WaitForSeconds(0.1f);
        
        for(int i = 0; i < tpings.Count; i++)
        {
            Destroy(tpings[i].gameObject);
            _allTpings.Remove(tpings[i]);
        }

        tpings.Clear();
        
        yield break;
    }

    // 점수 증가
    private void AddScore(int tpingCount)
    {
        _Score += (int)(tpingCount * 100 * (1 + (tpingCount - 3) * 0.1f));
    }
    
    // 횟수 감소
    private void SubsDragPoint()
    {
        if (DragPoint > 0 && CalDragPoint == 1)
            DragPoint--;

        DragPointText.text = DragPoint.ToString();
    }

    public void TargetClear(List<Tping> tpings)
    {
        foreach(var tpingItem in tpings)
        {
            var index = tpingItem.ID;

            if (targetIndex[index] > 0)
                targetIndex[index]--;

            for (int i = 0; i < Targets_Top.Length; i++)
            {
                int id = Targets_Top[i].GetComponent<Target>().Target_ID;

                Targets_Top[i].GetComponentInChildren<TextMeshProUGUI>().text = targetIndex[id].ToString();
            }
            GameClearCheck();

        }
    }

    private void GameClearCheck()
    {
        int remain = targetIndex.Length;

        for (int i = 0; i < targetIndex.Length; i++)
        {
            if (targetIndex[i] <= 0)
                remain--;
        }

        if (remain <= 0)
        {
            _isPlaying = false;
            _isClear = true;
            InGamePopUpBG.SetActive(true);
            InGamePopUps[2].SetActive(true);
            PlayerPrefs.SetInt("ClearCheck", 1);
            PlayerPrefs.SetFloat("CurrentScore", (float)_Score);

            if ((level + 1) > PlayerPrefs.GetInt("BestLevel"))
            {
                PlayerPrefs.SetInt("BestLevel", (level + 1));
                PlayerPrefs.SetInt("CurrentClearLevel", level);
            }
            else
            {
                PlayerPrefs.SetInt("CurrentClearLevel", level);
            }
            remain = 0;
        }
        
        if (DragPoint == 0 && remain > 0)
        {
            _isPlaying = false;
            InGamePopUpBG.SetActive(true);
            InGamePopUps[1].SetActive(true);
        }
    }
    #endregion

    #region 인게임 팝업

    IEnumerator TargetRemind()
    {
        InGamePopUpBG.SetActive(true);
        InGamePopUps[0].SetActive(true);

        yield return new WaitForSeconds(2f);
        
        InGamePopUpBG.SetActive(false);
        InGamePopUps[0].SetActive(false);
        
        TPingSpawn(60);
    }

    public void BacktoLevelSelectMap(int state)
    {
        PlayerPrefs.SetInt("InGameState", 0);
        PlayerPrefs.SetInt("StageClear", state);
        // 0: 디폴트 1: Clear 2: Lose
        SceneManager.LoadScene("Scenes_P/InGameScene");
    }

    public void QuitPopUp()
    {
        InGamePopUpBG.SetActive(false);
        for (int i = 0; i < InGamePopUps.Length; i++)
        {
            InGamePopUps[i].SetActive(false);
        }
    }

    public void PauseMenu()
    {
        InGamePopUpBG.SetActive(true);
        InGamePopUps[3].SetActive(true);
    }

    public void QuitCheckPopUp()
    {
        InGamePopUps[3].SetActive(false);
        InGamePopUps[4].SetActive(true);
    }
    
    

    #endregion
}
