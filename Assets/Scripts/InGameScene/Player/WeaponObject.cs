// Copyright 2013-2022 AFI, INC. All rights reserved.

using UnityEngine;

namespace InGameScene {
    //===============================================================
    // 둥둥 떠다니는 총 관련 클래스
    //===============================================================
    public class WeaponObject : MonoBehaviour {

    private EnemyObject _enemy; // 지정중인 적
    
    private Transform _gunSpriteTransform; // 무기의 이미지 Transform

    private float _reloadingTime = 3; // 리로딩 시간

    private GameObject _bulletObject; // 발사할 총알 객체
    
    private float _currentTime = 0; // 다음 총알까지 시간

    private Sprite _bulletImageSprite;
    // Start is called before the first frame update
    
    private BackendData.GameData.WeaponInventory.Item _weaponData;
    
    // 나의 무기 데이터를 이용하여 무기 스펙이 도입
    public void ActiveGun(BackendData.GameData.WeaponInventory.Item weaponInventoryData, GameObject bulletObject) {
        _weaponData = weaponInventoryData;

        if (_gunSpriteTransform == null) {
            _gunSpriteTransform = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
        }

        _gunSpriteTransform.GetComponent<SpriteRenderer>().sprite =
            weaponInventoryData.GetWeaponChartData().WeaponSprite;

        _bulletImageSprite = weaponInventoryData.GetWeaponChartData().BulletSprite;
        
        gameObject.SetActive(true);
        _bulletObject = bulletObject;
    }

    // 무기 해제 및 비활성화
    public void ReleaseGun() {
        if (_gunSpriteTransform == null) {
            _gunSpriteTransform = gameObject.GetComponentInChildren<SpriteRenderer>().transform;
        }

        _gunSpriteTransform.GetComponent<SpriteRenderer>().sprite = null;
        
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (_enemy == null) {
            return;
        }

        RotateToEnemyUpdate();
        ShootUpdate();

    }

    // 무기와 적의 각도를 계산하여 겨누는 함수
    void RotateToEnemyUpdate() {
        Vector3 relativePos = _enemy.transform.position - _gunSpriteTransform.position;
        Vector3 quaternionToTarget = Quaternion.Euler(0, 0, 0) * relativePos;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(0,0,1), upwards: quaternionToTarget);
        _gunSpriteTransform.rotation = targetRotation;
    }

    // 다음 리로딩까지 시간을 계산하는 함수
    // 
    void ShootUpdate() {
        _currentTime += Time.deltaTime;

        if (_currentTime > _reloadingTime) {
            Shoot();
            _currentTime = 0;
        }
    }

    // 무기를 겨눌 적을 지정하는 함수
    public void SetEnemy(EnemyObject enemyItem) {
        _enemy = enemyItem;
    }

    // 총을 발사하는 함수
    void Shoot() {
        
        //총알의 스피드
        float speed = _weaponData.GetCurrentWeaponStat().Spd;

        // 버프받은 만큼 공격력 강화
        float normalAtk = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Atk, _weaponData.GetCurrentWeaponStat().Atk);
        
        // 리로딩 타음 또한 버프만큼 계산
        _reloadingTime = Managers.Buff.GetBuffedStat(Buff.BuffStatType.Delay, _weaponData.GetCurrentWeaponStat().Delay);
        
        // 총알 발사
        var bullet = Instantiate(_bulletObject);
        bullet.GetComponent<BulletObject>().Shoot(_bulletImageSprite, _gunSpriteTransform.rotation, speed, normalAtk);
        bullet.transform.position = _gunSpriteTransform.position;
    }
}
}
