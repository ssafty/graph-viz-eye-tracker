using UnityEngine;    // For Debug.Log, etc.

using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System;
using System.Runtime.Serialization;
using System.Reflection;

// === This is the info container class ===
[Serializable()]
public class Settings : ISerializable
{

    // === Values ===
    // Edit these during gameplay
    public float xDeg = 0;
    public float yDeg = 0;
    public float zDeg = 0;
    // === /Values ===

    // The default constructor. Included for when we call it during Save() and Load()
    public Settings() { }

    // This constructor is called automatically by the parent class, ISerializable
    // We get to custom-implement the serialization process here
    public Settings(SerializationInfo info, StreamingContext ctxt)
    {
        // Get the values from info and assign them to the appropriate properties. Make sure to cast each variable.
        // Do this for each var defined in the Values section above
        xDeg = (float)info.GetValue("xDeg", typeof(float));
        yDeg = (float)info.GetValue("yDeg", typeof(float));
        zDeg = (float)info.GetValue("zDeg", typeof(float));
    }

    // Required by the ISerializable class to be properly serialized. This is called automatically
    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
        // Repeat this for each var defined in the Values section
        info.AddValue("xDeg", xDeg);
        info.AddValue("yDeg", yDeg);
        info.AddValue("zDeg", zDeg);
    }
}

// === This is the class that will be accessed from scripts ===
public class SaveLoad
{

    public static string currentFilePath = "MuCDemo.settings";    // Edit this for different save files

    // Call this to write data
    public static void Save(Settings data)  // Overloaded
    {
        Save(currentFilePath, data);
    }
    public static void Save(string filePath, Settings data)
    {
        Stream stream = File.Open(filePath, FileMode.Create);
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        bformatter.Serialize(stream, data);
        stream.Close();
        Debug.Log("Settings serialized!");
    }

    // Call this to load from a file into "data"
    public static Settings Load() { return Load(currentFilePath); }   // Overloaded
    public static Settings Load(string filePath)
    {
        Settings data = new Settings();
        Stream stream = File.Open(filePath, FileMode.Open);
        BinaryFormatter bformatter = new BinaryFormatter();
        bformatter.Binder = new VersionDeserializationBinder();
        data = (Settings)bformatter.Deserialize(stream);
        stream.Close();

        return data;
        // Now use "data" to access your Values
    }

}

// === This is required to guarantee a fixed serialization assembly name, which Unity likes to randomize on each compile
// Do not change this
public sealed class VersionDeserializationBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
        {
            Type typeToDeserialize = null;

            assemblyName = Assembly.GetExecutingAssembly().FullName;

            // The following line of code returns the type. 
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));

            return typeToDeserialize;
        }

        return null;
    }
}
