using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using TMPro;
using UnityEngine.Serialization;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData levelDB;
    
    // 전체 티니핑 관리 배열
    public List<Tping> _allTpings   = new List<Tping>();
    // 티니핑 선택 배열
    public 
        List<Tping> _selectPings = new List<Tping>();
    // 타겟 정보
    private Target _target;
    // 체인 블록
    private Bomb   _bomb;
    // 티니핑 인덱스 검색인자 ( 추후 형 변환 리팩토링할것)
    private string _selectID;
    // 플레이 체크
    private bool   _isPlaying = true;
    // 승리 체크
    private bool   _isClear = false;
    // 점수 값
    private int    _Score = 0;

    private int level;
    
    private LevelSort _sort;

    private LevelActive _levelActive;

    // 싱글톤 사용
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    // 티니핑 프리팹 배열 ( 현재 샘플로 과일 사용중 )
    [SerializeField]
    private GameObject[]    TPingPrefabs;
    // 라인 렌더러
    [SerializeField]
    private LineRenderer    LineRenderer;
    // 폭탄 프리팹
    [SerializeField]
    private GameObject      BombPrefab;
    // 게임 오버
    [SerializeField]
    private GameObject      FinishDialog;
    // 클리어 화면
    [SerializeField]
    private GameObject      ClearDialog;
    // 리미트 초기화 값
    [SerializeField]
    private TextMeshProUGUI  DragPointText;

    // 최소 연결 횟수
    private int          TpingDestroyCount = 3;
    // 연결 범위
    private float        TpingConnectRange = 1f;
    // 체인블록 최소 생성 카운트
    private int          BombSpawnCount    = 5;
    // 폭발 범위
    private float        BombDestroyRange  = 1.5f;
    // 제한 횟수
    private int          DragPoint;
    // 연결 사용 감소 횟수
    private int          CalDragPoint      = 1;

    [Header("목표 값 프리팹")]
    [SerializeField]
    private GameObject[] Targets;
    
    [Header("목표 값 설정")]
    [SerializeField]
    private int[] targetIndex = new int[6];

    // 나중에 스테이지 시작 함수로 변경할것
    void Awake()
    {
        level = PlayerPrefs.GetInt("SelectedLevel");
        Instance = this;
        // 시작시 생성량
        TPingSpawn(40);
        DragPointText.text = levelDB.Sheet1[level].drag.ToString();
        
        string[] targets = levelDB.Sheet1[level].targets.Split(", ");
        string[] targetValues = levelDB.Sheet1[level].targetValue.Split(", ");
        
        for (int i = 0; i < targets.Length; i++)
        {
            int result;
            int.TryParse(targets[i], out result);

            int result2;
            int.TryParse(targetValues[i], out result2);
            
            targetIndex[result] = result2;
            
            int id = Targets[result].GetComponent<Target>().Target_ID;
            
            Targets[result].SetActive(true);
            Targets[result].GetComponentInChildren<TextMeshProUGUI>().text = targetIndex[id].ToString();
            Targets[result].GetComponent<Target>()._TargetAnim.SetInteger
                ("ID", Targets[i].GetComponent<Target>().Target_ID);
        }
    }
    
    // 업데이트 값 [ 시간, 라인렌더러 ]
    void Update()
    {
        LineRendererUpdate();
    }
    
    // 라인 렌더러 설정
    private void LineRendererUpdate()
    {
        if (_selectPings.Count >= 2)
        {
            LineRenderer.positionCount = _selectPings.Count;

            LineRenderer.SetPositions(_selectPings.Select(tping => tping.transform.position).ToArray());
            
            LineRenderer.gameObject.SetActive(true);
        }
        else LineRenderer.gameObject.SetActive(false);
    }
    
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
        Instantiate(BombPrefab, _selectPings[_selectPings.Count -1].transform.position, Quaternion.identity);
        Bomb.BInstance._bombAnim.SetInteger("Bomb_num", num);
        Bomb.BInstance.bomb_num = num;
    }
    
    
    #region 마우스 인풋 함수
    
    // 클릭하여 선택 시작
    public void PingDown(Tping tping)
    {
        // 예외처리
        if (!_isPlaying) return;
        
        // 선택된 티니핑들을 tping 값에 넣음
        _selectPings.Add(tping);
        tping.SetIsSelect(true);
        
        _selectID = tping.ID;
    }
    
    // 드래그
    public void PingEnter(Tping tping)
    {
        // 예외처리
        if (!_isPlaying) return;
        if (_selectID != tping.ID) return;
        
        // 티니핑 선택시
        if (tping.IsSelect)
        {
            if (_selectPings.Count >= 2 && _selectPings[_selectPings.Count - 2] == tping)
            {
                var RemoveTping = _selectPings[_selectPings.Count - 1];
                RemoveTping.SetIsSelect(false);
                _selectPings.Remove(RemoveTping);
            }
        }
        // 주변 선택시
        else
        {
            var length = (_selectPings[_selectPings.Count - 1].transform.position
                          - tping.transform.position).magnitude;
            if (length < TpingConnectRange)
            {
                _selectPings.Add(tping);
                tping.SetIsSelect(true);
            }
        }
    }
    
    // 선택 종료
    public void PingUp()
    {
        if (!_isPlaying) return;

        int selectN = _selectPings.Count;
        
        if (selectN >= TpingDestroyCount)
        {
            SubsDragPoint(CalDragPoint);

            foreach (var index in _selectPings)
            {
                index.clearAnim.SetBool("Cleared", true);
                index.clearEffect.SetActive(true);
            }

            for (int i = 3; i > 0; i--)
            {
                if (selectN >= BombSpawnCount + (i - 1))
                {
                    MakeBomb(i, (0.3f * i + 2));
                    break;
                }

            }
            // 그 이하는 체인블록 미생성, 연결 블록만 제거
            TargetClear(_selectPings);
            DestroyTpings(_selectPings);
        }       
        else
        {
            foreach (var tpingItem in _selectPings)
                tpingItem.SetIsSelect(false);
        }

        _selectID = "";
        _selectPings.Clear();
    }
    
    // 체인블록 사용
    public void BombDown(Bomb bomb)
    {
        // 플레이 체크
        if (!_isPlaying) return;
        
        var RemoveTpings = new List<Tping>();
        
        foreach (var TpingItem in _allTpings)
        {
            var Length = (TpingItem.transform.position - bomb.transform.position).magnitude;
            // 체인블록 상태 검증 [ 크기, 특수블록 레벨 ]
            switch (bomb.bomb_num)
            {
                case 1:
                    if (Length < (BombDestroyRange + bomb.level))
                        RemoveTpings.Add(TpingItem);
                    break;
                case 2:
                    if (Length < (BombDestroyRange + bomb.level))
                        RemoveTpings.Add(TpingItem);
                    break;
                case 3:
                    if (Length < (BombDestroyRange * 1.5f) + bomb.level)
                        RemoveTpings.Add(TpingItem);
                    break;
            }
        }
        
        TargetClear(RemoveTpings);
        DestroyTpings(RemoveTpings);
        Destroy(bomb.gameObject);
    }
    
    // 블록 파괴시 실행 (점수 증가, 블록 재생성 관리 )
    private void DestroyTpings(List<Tping> tpings)
    {
        foreach (var tpingItem in tpings)
        {
            Destroy(tpingItem.gameObject);
            _allTpings.Remove(tpingItem);
        }
        
        TPingSpawn(tpings.Count);
        AddScore(tpings.Count);
    }
    
    // 점수 증가
    private void AddScore(int tpingCount)
    {
        _Score += (int)(tpingCount * 100 * (1 + (tpingCount - 3) * 0.1f));
    }
    
    // 횟수 감소
    private void SubsDragPoint(int dragCount)
    {
        if (DragPoint > 0 && CalDragPoint == 1)
            DragPoint -= dragCount;
        else if (DragPoint > 0 && CalDragPoint != 1)
            DragPoint += dragCount;
        
        DragPointText.text = DragPoint.ToString();
    }
    
    public void TargetClear(List<Tping> tpings)
    {
        foreach(var tpingItem in tpings)
        {
            var index = int.Parse(tpingItem.ID);

            if (targetIndex[index] > 0)
                targetIndex[index]--;

            for (int i = 0; i < Targets.Length; i++)
            {
                int id = Targets[i].GetComponent<Target>().Target_ID;

                Targets[i].GetComponentInChildren<TextMeshProUGUI>().text = targetIndex[id].ToString();
            }
            GameClearCheck();

        }
    }

    private void GameClearCheck()
    {
        int remain = 0;

        for (int i = 0; i < targetIndex.Length; i++)
        {
            if (targetIndex[i] <= 0)
                remain++;
        }

        if (remain >= 8)
        {
            _isPlaying = false;
            _isClear = true;
            ClearDialog.SetActive(true);
            PlayerPrefs.SetFloat("CurrentScore", (float)_Score);
        }
        else if (DragPoint == 0 && remain > 0)
        {
            _isPlaying = false;
            FinishDialog.SetActive(true);
        }
        else
            remain = 0;
    }
    #endregion
}
