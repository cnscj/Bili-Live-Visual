using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class H_Descation
{
    public static Dictionary<int, string> formaMap = new Dictionary<int, string>()
    {
        [10001] = "{0} Test",

        [10101] = "{0} Test",

        [10201] = "��ʾ��Ŀ����",
        [10202] = "��ʾ��Ļ",
        [10203] = "������Ŀ����",
        [10204] = "���ص�Ļ",

        [10301] = "{0}",
        [10302] = "{0}��SC:{1}",
        [10303] = "{0}{1}{2}", //Ͷι����
        [10304] = "{0}{1}{2}",  //�Ͻ�
    };
}
