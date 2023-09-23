using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ego : ∆Ú≈∏


public class SkillController : BaseController
{
    public Define.SKillType SkillType { get; set; }
    public Data.SkillData SkillData { get; protected set; }

    #region Destroy
    Coroutine _coDestory;

    public void StartDestroy(float delaySeconds)
    {
        StopDestroy();
        _coDestory = StartCoroutine(CoDestroy(delaySeconds));
    }

    public void StopDestroy()
    {
        if(_coDestory != null)
        {
            StopCoroutine(_coDestory);
            _coDestory = null;
        }
    }

    IEnumerator CoDestroy(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        if (this.IsValid())
        {
            Managers.Object.Despawn(this);
        }
    }
    #endregion
}
