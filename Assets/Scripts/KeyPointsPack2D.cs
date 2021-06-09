using System;
using UnityEngine;

namespace DefaultNamespace
{
    [System.Serializable]
    public class KeyPointsPack2D
    {
        public int frame_in_seq = -1;
        public float[] x_poses = new float[17];
        public float[] y_poses = new float[17];

        private KeyPointsPack2D(int frm_no, float[] _xposes, float[] _yposes)
        {
            frame_in_seq = frm_no;
            x_poses = _xposes;
            y_poses = _yposes;
        }
        
        public static KeyPointsPack2D GetPackFromJSON(string json)
        {
            var obj = JsonUtility.FromJson<KeyPointsPack2D>(json);
            return obj;
        }

        public static KeyPointsPack2D GetPackCommaSeparatedString(string csvStr)
        {
            string[] strs = csvStr.Split(',');
            if (strs.Length != 35)
            {
                throw new Exception("The comma separated string must consist of 35 string segments.");
            }

            var frm_no = int.Parse(strs[0]);
            float[] x_poses_float = new float[17];
            float[] y_poses_float = new float[17];
            for (int i = 0; i < 17; i++)
            {
                x_poses_float[i] = float.Parse(strs[i + 1]);
                y_poses_float[i] = float.Parse(strs[i + 1 + 17]);
            }
            return new KeyPointsPack2D(frm_no, x_poses_float, y_poses_float);
        }
        
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < x_poses.Length; i++)
            {
                str += String.Format("<< {0} === {1} >>\n",x_poses[i],y_poses[i]);
            }
            return str;
        }
    }
}