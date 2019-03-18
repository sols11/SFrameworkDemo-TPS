using UnityEngine;
using SFramework;
using UnityEngine.UI;

namespace ProjectScript
{
    public class UIAimTarget : ViewBase
    {
        private void Awake()
        {
            base.UIForm_Type = UIFormType.Normal;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Lucency;
        }

        private void Start()
        {
        }

    }
}