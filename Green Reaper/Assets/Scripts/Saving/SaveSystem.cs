using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(int coins, int quest, string saveName, int[] upgrades, bool questCompletion)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = GenerateSavePath(saveName);

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            SaveData data = new SaveData(coins, quest, upgrades, questCompletion);
            formatter.Serialize(stream, data);
        }
    }

    public static SaveData LoadGame(string saveName)
    {
        string path = GenerateSavePath(saveName);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SaveData data;

            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                 data = formatter.Deserialize(stream) as SaveData;
            }

            return data;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Deletes the file at the given save name.
    /// </summary>
    /// <param name="saveName">Name of the save to delete.</param>
    public static void DeleteGame(string saveName)
    {
        string path = GenerateSavePath(saveName);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string GenerateSavePath(string saveName)
    {
        string path = Application.persistentDataPath + "/"+ saveName + ".save";
        return path;
    }
}
