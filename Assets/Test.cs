using System;
using KZLib.KZData;
using KZLib.KZUtility;
using MessagePack;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using YamlDotNet.Serialization;

public class OptionConfig
{
    public string ProtoFolderPath { get; private set; }
    public Vector3Int ProtoPos { get; private set; }
    public Color ProtoColor { get; private set; }
    public SoundVolume ProtoVolume { get; private set; }

    public OptionConfig() { }

    public OptionConfig(string tt)
    {
        ProtoFolderPath = "Resources/Text/Proto";
        ProtoPos = new Vector3Int(1,2,3);
        ProtoColor = Color.green;
        ProtoVolume = new SoundVolume(0.7f,true);
    }
}

public class Test : MonoBehaviour
{
	[Button("AddT")]
    private void AddTest()
    {
        // var test1 = 0L;
        // var test2 = test1.AddFlag(GraphicsQualityOption.GLOBAL_TEXTURE_MIPMAP_LIMIT);  //1
        // var test3 = test2.AddFlag(GraphicsQualityOption.ANISOTROPIC_FILTERING);  //1
        // var test4 = test3.AddFlag(GraphicsQualityOption.VERTICAL_SYNC_COUNT); //0
        // var test5 = test4.AddFlag(GraphicsQualityOption.CAMERA_FAR_HALF);

        // LogTag.Network.I(Convert.ToString(test1,2));
        // LogTag.Network.I(Convert.ToString(test2,2));
        // LogTag.Network.I(Convert.ToString(test3,2));
        // LogTag.Network.I(Convert.ToString(test4,2));
        // LogTag.Network.I(Convert.ToString(test5,2));

        // Convert.ToString(value, 2);


    }
}