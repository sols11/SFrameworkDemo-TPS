using UnityEngine;
using SFramework;

namespace ProjectScript
{
    public class UICrosshair : ViewBase
    {
        public float length = 7;         // 准星的长度
        public float width = 2;          // 准星的宽度
        public float distance = 15;      // 准星的距离
        public Texture2D white;
        public Texture2D red;

        private GUIStyle lineStyle;
        private Texture texture;

        private void Awake()
        {
            base.UIForm_Type = UIFormType.Fixed;
            base.UIForm_ShowMode = UIFormShowMode.Normal;
            base.UIForm_LucencyType = UIFormLucenyType.Pentrate;
            lineStyle = new GUIStyle();
            lineStyle.normal.background = white;
        }

        private void OnGUI()
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == (int)ObjectLayer.Enemy)
                    lineStyle.normal.background = red;
                else
                    lineStyle.normal.background = white;
            }

            // 左准星线
            GUI.Box(new Rect((Screen.width - distance) / 2 - length, (Screen.height - width) / 2, length, width), texture, lineStyle);
            // 右准星线
            GUI.Box(new Rect((Screen.width + distance) / 2, (Screen.height - width) / 2, length, width), texture, lineStyle);
            // 上准星线
            GUI.Box(new Rect((Screen.width - width) / 2, (Screen.height - distance) / 2 - length, width, length), texture, lineStyle);
            // 下准星线
            GUI.Box(new Rect((Screen.width - width) / 2, (Screen.height + distance) / 2, width, length), texture, lineStyle);
        }
    }
}