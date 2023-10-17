using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SkillCardItem : UI_Base
{
    //� ��ų?
    //�� ����?
    //����Ʈ ��Ʈ?
    int _tempalteID;
    Data.SkillData _skillData;

    public void SetInfo(int templateID)
    {
        _tempalteID = templateID;

        Managers.Data.SkillDic.TryGetValue(templateID, out _skillData);
    }

    public void OnClickItem()
    {
        //��ų ���� ���׷��̵�
        Debug.Log("OnClickItem");
        Managers.UI.ClosePopup();
    }
}
