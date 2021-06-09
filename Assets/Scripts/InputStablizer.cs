using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// The input data that is extracted from the PythonPortal can be corrupt, noisy,
    /// and full of jitterings. This Singleton class provides necessary functions
    /// for stabilization of the input data.
    /// </summary>
    public class InputStablizer
    {
        private static InputStablizer globalInstance;
        public static InputStablizer mainStablizer
        {
            get
            {
                if (globalInstance == null)
                {
                    globalInstance = new InputStablizer();
                }
                return globalInstance;
            }
        }

        private Vector2 leftHandPos;
        private Vector2 rightHandPos;
        private Vector2 headPos;

        private InputStablizer()
        {
            headPos = new Vector3(0.5f, 0.9f, 0.0f);
            rightHandPos = new Vector3(0.8f, 0.5f, 0.0f);
            leftHandPos = new Vector3(0.2f, 0.5f, 0.0f);
        }
        /// <summary>
        /// The keypoints data that is collected from the PythonPortal is sent
        /// to this object for further process via this method.
        /// </summary>
        /// <param name="kp2d">The object instance of type "KeyPointsPack2D"</param>
        public void UpdateKeyPointsPack2D(KeyPointsPack2D kp2d)
        {
            float[] xposes = kp2d.x_poses;
            float[] yposes = kp2d.y_poses;
            float eps = 0.01f;
            headPos.x = xposes[0];
            headPos.y = yposes[0];
            leftHandPos.x = xposes[9];
            leftHandPos.y = yposes[9];
            rightHandPos.x = xposes[10];
            rightHandPos.y = yposes[10];
        }

        public Vector3 GetHeadPos()
        {
            return headPos;
        }

        public Vector3 GetRightHandPos()
        {
            return rightHandPos;
        }

        public Vector3 GetLeftHandPos()
        {
            return leftHandPos;
        }
    }
}