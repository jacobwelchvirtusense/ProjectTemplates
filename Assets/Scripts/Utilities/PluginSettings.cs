/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/10/2023 10:02:13 AM
 * 
 * Description: Gets data about the current plugin/kinnect sensor
 *              being used.
******************************************************************/
using System.IO;
using System.Xml;

/// <summary>
/// The type of sensor being used.
/// </summary>
public enum SensorType
{
    KINNECTV2,
    ASTRAPRO,
    AZUREKINNECT
}

internal class PluginSettings
{
    /// <summary>
    /// The string defining this user as using the kinnect2 sensor.
    /// </summary>
    private const string KinectPlugin = "kinect2plugin";

    /// <summary>
    /// The string defining this user as using the azure kinnect sensor.
    /// </summary>
    private const string AzureKinectPlugin = "azurekinectplugin";

    /// <summary>
    /// The string defining this user as using the orbbec sensor.
    /// </summary>
    private const string OrbbecPlugin = "orbbecPlugin";

    /// <summary>
    /// The current type of sensor being used.
    /// </summary>
    public SensorType SensorType { get; internal set; } = SensorType.KINNECTV2;

    /// <summary>
    /// Checks which sensor is currently being used.
    /// </summary>
    public void LoadPluginSettings()
    {
        try
        {
            using (StreamReader input = new StreamReader("VSClientSettings.xml"))
            {
                var xmlDoc = Parse(input.ReadToEnd());
                XmlNodeList plugin_nodes = xmlDoc.SelectNodes("/configuration/plugins/plugin");

                foreach (XmlNode node in plugin_nodes)
                {
                    var name = node.SelectSingleNode("name");
                    if (name == null) continue;
                    
                    var plugin_name = name.InnerText;

                    if (plugin_name.ToLower() == "kinect2")
                    {
                        var fully_qualified_name = node.SelectSingleNode("type").InnerText;
                        var parts = fully_qualified_name.Split('.');
                        var adaptername = parts == null || parts.Length == 0 ? "" : parts[parts.Length - 1];
                        
                        switch (adaptername.ToLower())
                        {
                            case OrbbecPlugin:
                                SensorType = SensorType.ASTRAPRO;
                                break;
                            case AzureKinectPlugin:
                                SensorType = SensorType.AZUREKINNECT;
                                break;
                            case KinectPlugin:
                            default:
                                SensorType = SensorType.KINNECTV2;
                                break;
                        }
                    }
                }
            }
        }
        catch (IOException)
        {
            
        }
    }

    /// <summary>
    /// Parses the XML document.
    /// </summary>
    /// <param name="xmlString">The string of the document to be parsed.</param>
    /// <returns></returns>
    public static XmlNode Parse(string xmlString)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xmlString);

        return doc.DocumentElement;
    }
}

