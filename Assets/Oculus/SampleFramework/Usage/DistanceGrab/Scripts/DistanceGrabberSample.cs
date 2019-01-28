/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
    public class DistanceGrabberSample : MonoBehaviour
    {

        bool m_useSpherecast = false;

        public bool UseSpherecast
        {
            get { return m_useSpherecast; }
            set
            {
                m_useSpherecast = value;
                for (int i = 0; i < m_grabbers.Length; ++i)
                {
                    m_grabbers[i].UseSpherecast = m_useSpherecast;
                }
            }
        }

        bool m_allowGrabThroughWalls = false;
        public bool AllowGrabThroughWalls
        {
            get { return m_allowGrabThroughWalls; }
            set
            {
                m_allowGrabThroughWalls = value;
                for (int i = 0; i < m_grabbers.Length; ++i)
                {
                    m_grabbers[i].m_preventGrabThroughWalls = !m_allowGrabThroughWalls;
                }
            }
        }

        [SerializeField]
        DistanceGrabber[] m_grabbers;

        // Use this for initialization
        void Start()
        {
            DebugUIBuilder.instance.AddLabel("Distance Grab Sample");
            DebugUIBuilder.instance.AddToggle("Use Spherecasting", ToggleSphereCasting, m_useSpherecast);
            DebugUIBuilder.instance.AddToggle("Grab Through Walls", ToggleGrabThroughWalls, m_allowGrabThroughWalls);
            DebugUIBuilder.instance.Show();
        }

        public void ToggleSphereCasting(Toggle t)
        {
            UseSpherecast = !UseSpherecast;
        }

        public void ToggleGrabThroughWalls(Toggle t)
        {
            AllowGrabThroughWalls = !AllowGrabThroughWalls;
        }
    }
}
