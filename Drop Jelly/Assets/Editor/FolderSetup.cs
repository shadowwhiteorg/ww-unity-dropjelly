using UnityEditor;
using UnityEngine;
using System.IO;

public class FolderSetup
{
    [MenuItem("Tools/Create Project Folders")]
    private static void CreateFolders()
    {
        string[] folders = new string[]
        {
            "Assets/Art/UI/Icons",
            "Assets/Art/UI/Menus",
            "Assets/Art/UI/Overlays",
            "Assets/Art/Materials",
            "Assets/Art/Textures",
            "Assets/Audio/Music",
            "Assets/Audio/SFX",
            "Assets/Prefabs",
            "Assets/Resources",
            "Assets/Scenes/Development",
            "Assets/Scenes/Release",
            "Assets/Scripts/Core/InputHandling",
            "Assets/Scripts/Core/Utilities",
            "Assets/Scripts/Managers",
            "Assets/Scripts/Main",
            "Assets/Scripts/SOScripts",
            "Assets/Scripts/Tools/ScriptableObjects",
            "Assets/Scripts/Tools/CustomInterfaces",
            "Assets/Scripts/UI/Controls",
            "Assets/Scripts/UI/HUD",
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parentFolder = Path.GetDirectoryName(folder);
                string newFolder = Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parentFolder, newFolder);
            }

            string readmePath = Path.Combine(folder, "readit.md");
            if (!File.Exists(readmePath))
            {
                string folderName = Path.GetFileName(folder);
                string readmeContent = $"# {folderName}\n\nThis folder contains assets related to {folderName}.";

                // Create the directory if it doesn't exist
                Directory.CreateDirectory(Path.GetDirectoryName(readmePath));

                File.WriteAllText(readmePath, readmeContent);
            }
        }

        AssetDatabase.Refresh();
    }
}
