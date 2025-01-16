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
        // OptionConfig 객체를 JSON으로 직렬화하여 저장
        var config = new OptionConfig("Resources/Text/Proto");

        var json = JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, });

        LogTag.Build.I(json);

        var configJson = JsonConvert.DeserializeObject<OptionConfig>(json);

        LogTag.Build.I(configJson.ProtoFolderPath);
        LogTag.Build.I(configJson.ProtoPos);
        LogTag.Build.I(configJson.ProtoColor);
        LogTag.Build.I(configJson.ProtoVolume);

        var yamlSerializer = new SerializerBuilder().WithTypeConverter(new YamlConverter()).Build();

        var yaml = yamlSerializer.Serialize(config);

        LogTag.Network.I(yaml);

        var yamlDeserializer = new DeserializerBuilder().WithTypeConverter(new YamlConverter()).Build();

        var configYaml = yamlDeserializer.Deserialize<OptionConfig>(yaml);

        LogTag.Network.I(configYaml.ProtoFolderPath);
        LogTag.Network.I(configYaml.ProtoPos);
        LogTag.Network.I(configYaml.ProtoColor);
        LogTag.Network.I(configYaml.ProtoVolume);
    }
}