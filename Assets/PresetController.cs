using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class PresetController : MonoBehaviour
{

    [System.Serializable]
    public class KeyboardMapping
    {
        public int index;
        public string key;

    }

    [System.Serializable]
    public class PresetData
    {
        public Vector4 CloudPosition;
        public Vector4 CloudScale;
        public Quaternion CloudRotation;
        public Vector4 FractalA;
        public Vector4 FractalB;
        public Vector4 FractalC;
        public Vector4 RenderParams;
        public Vector4 RaymarchParams;
    }


    [SerializeField]
    MultibufferParticles cloudObject;

    [SerializeField]
    Vector4Shader m_renderParams;

    [SerializeField]
    Vector4Shader m_raymarchParams;

    [SerializeField]
    Vector4Shader m_FractalA;
    [SerializeField]
    Vector4Shader m_FractalB;
    [SerializeField]
    Vector4Shader m_FractalC;

    [SerializeField]
    public List<PresetData> m_presets;

    [SerializeField]
    public List<GameObject> mControls;

    [SerializeField]
    public List<KeyboardMapping> m_controllers;
    public int currentPreset = -1;

    void Start()
    {

        m_presets = new List<PresetData>();

        // parse the m_presets
        Debug.LogFormat("Loading {0}", mappingsFileName());
        FileInfo theSourceFile = new FileInfo(mappingsFileName());
        StreamReader reader = theSourceFile.OpenText();
        string text;
        do
        {
            text = reader.ReadLine();
            KeyboardMapping p = JsonUtility.FromJson<KeyboardMapping>(text);
            if ( p != null )
            {
                m_controllers.Add(p);
            }
            Debug.Log(text);
        } while (text != null);

        Debug.LogFormat("Loading {0}", presetFileName());
        theSourceFile = new FileInfo(presetFileName());
        reader = theSourceFile.OpenText();

        do
        {
            text = reader.ReadLine();
            PresetData p = JsonUtility.FromJson<PresetData>(text);
            m_presets.Add(p);
            Debug.Log(text);
        } while (text != null);
    }

    public string presetFileName()
    {
        return Application.dataPath + "/presets.txt";
    }

    public string mappingsFileName()
    {
        return Application.dataPath + "/mappings.txt";
    }
    public void Next()
    {
        currentPreset = (currentPreset + 1) % m_presets.Count;
        loadPreset(currentPreset);
    }

    public void DeleteCurrent()
    {
        m_presets.RemoveAt(currentPreset);
        OverwritePresetFile();
    }

    public void Last()
    {
        currentPreset = (currentPreset - 1 +  m_presets.Count) % m_presets.Count;
        loadPreset(currentPreset);
    }

    private void SaveMappings()
    {
        File.Delete(mappingsFileName());
        foreach (KeyboardMapping p in m_controllers)
        {
            string text = JsonUtility.ToJson(p);
            text += "\n";
            System.IO.File.AppendAllText(mappingsFileName(), text);
        }
    }
    private void OverwritePresetFile()
    {
        File.Delete(presetFileName());
        foreach (PresetData p in m_presets)
        {
            string text = JsonUtility.ToJson(p);
            text += "\n";
            System.IO.File.AppendAllText(presetFileName(), text);
        }

    }

    void loadPreset(int index)
    {
        PresetData data = m_presets[index];
        m_FractalA.Vec4 = data.FractalA;
        m_FractalB.Vec4 = data.FractalB;
        m_renderParams.Vec4 = data.RenderParams;
        m_raymarchParams.Vec4 = data.RaymarchParams;

        cloudObject.gameObject.transform.position = data.CloudPosition;
        cloudObject.gameObject.transform.localScale = data.CloudScale;
        cloudObject.gameObject.transform.rotation = data.CloudRotation;
    }

    public void SavePreset()
    {
        PresetData presetObject = new PresetData();

        presetObject.FractalA = m_FractalA.Vec4;
        presetObject.FractalB = m_FractalB.Vec4;
        presetObject.FractalC = m_FractalC.Vec4;
        presetObject.RenderParams = m_renderParams.Vec4;
        presetObject.RaymarchParams = m_raymarchParams.Vec4;

        presetObject.CloudPosition = cloudObject.gameObject.transform.position;
        presetObject.CloudScale = cloudObject.gameObject.transform.localScale;
        presetObject.CloudRotation = cloudObject.gameObject.transform.rotation;

        m_presets.Add(presetObject);

        OverwritePresetFile();

        SaveMappings();
    }
    private void HideControls()
    {
        foreach(GameObject obj in mControls)
        {
            obj.SetActive(!obj.activeInHierarchy);
        }
    }

    public void Update()
    {
        for (int i = 0; i < m_controllers.Count; i++)
            if (Input.GetKeyDown(m_controllers[i].key))
            {
                if (m_controllers[i].index == -1)
                {
					Next();
                }

                else if (m_controllers[i].index == -2)
                {
					Last();
                }
                else if (m_controllers[i].index == -3)
                {
					HideControls();
                }
                else
                {
                    loadPreset(m_controllers[i].index);
                }

            }
    }
}
