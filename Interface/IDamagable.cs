using UnityEngine;

public interface IDamagable
{
    /// <summary>
    /// 피격 시 호출되는 메서드입니다.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>피격 후 사망 여부</returns>
    bool GetDamaged(float damage);

    Vector3 GetDamagedPos();
}
