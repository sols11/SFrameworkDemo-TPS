/*----------------------------------------------------------------------------
Author:
    Anotts
Date:
    2017/08/01
Description:
    简介：游戏界面语言管理器
    作用：从设置获取当前语言，存储不同语言显示的字符，以及更换游戏中的字体
    使用：当需要显示文本时，调用ShowText接口
    补充：默认提供中文和英语两种语言选择
History:
----------------------------------------------------------------------------*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// 多语言本地化
    /// </summary>
    public class LanguageMgr : IGameMgr
    {
        private SettingData SettingSaveData { get; set; }    // 存储语言首选项
        private Dictionary<string, string> dicLauguageCN;
        private Dictionary<string, string> dicLauguageEN;
        private Font fontCN;
        private Font fontEN;

        public LanguageMgr(GameMainProgram gameMain) : base(gameMain)
        {
            dicLauguageCN = new Dictionary<string, string>();
            dicLauguageEN = new Dictionary<string, string>();
        }

        public override void Awake()
        {
            SettingSaveData = gameMain.gameDataMgr.SettingSaveData;
            // 加载语言本地化数据文件
            dicLauguageCN = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, string>>("Language_CN");
            dicLauguageEN = gameMain.fileMgr.LoadJsonDataBase<Dictionary<string, string>>("Language_EN");
            // 加载语言对应的字体
            fontCN = gameMain.resourcesMgr.LoadResource<Font>(@"Fonts\FZYH");
            fontEN = gameMain.resourcesMgr.LoadResource<Font>(@"Fonts\ARJULIAN");
        }

        /// <summary>
        /// 显示文本信息
        /// </summary>
        /// <param name="stringId">文本的ID</param>
        /// <returns></returns>
        public string ShowText(string stringId)
        {
            if (SettingSaveData.IsChinese)
                return UnityHelper.FindDic(dicLauguageCN, stringId);
            else
                return UnityHelper.FindDic(dicLauguageEN, stringId);
        }

        public Font GetFont(int fontChoose = 0)
        {
            if (fontChoose == 1)
                return fontCN;
            else if (fontChoose == 2)
                return fontEN;
            else if (SettingSaveData.IsChinese)
                return fontCN;
            else
                return fontEN;

        }
    }
}
