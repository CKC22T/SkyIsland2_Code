#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class SceneJumper : EditorWindow
{
    private static readonly string mSceneExtension = ".unity";

    private static string mAllSceneAbsolutePath => Application.dataPath;
    private static readonly string mAllSceneRelativePath = "Assets";

    private static string mExtensionFilePath => Path.Combine
    (
        GetThisCodeFileDirectory(),
        $"{GetThisCodeFileNameWithoutExtension()}{"Extension.cs"}"
    );

    public class ScenePathData
    {
        public string SceneAbsolutePath;
        public string SceneEditorPath;
        public string QuickSlotPath;
        public string FunctionSignature;
        public string SceneName;

        public ScenePathData(string sceneAbsolutePath, string relativeScenePath)
        {
            SceneName = Path.GetFileNameWithoutExtension(sceneAbsolutePath);

            string relativePath = Directory.GetParent(Application.dataPath).ToString();
            SceneAbsolutePath = sceneAbsolutePath.Replace("\\", "/");

            SceneEditorPath = PathExtension.GetRelativePath(relativePath, SceneAbsolutePath).Replace("\\", "/");
            QuickSlotPath = SceneEditorPath.Replace(relativeScenePath, "Jumper");
            QuickSlotPath = QuickSlotPath.Substring(0, QuickSlotPath.Length - mSceneExtension.Length);
            FunctionSignature = "ChangeTo_" + QuickSlotPath.Replace("/", "_");
            FunctionSignature = Regex.Replace(FunctionSignature, @" |\.|-|\(|\)|~", "_");
        }

        public string GetFunctionDeclaration(int priority = 0)
        {
            return $"\n\t[MenuItem(\"{QuickSlotPath}\", priority = {priority.ToString()})]\n" +
                $"\tprivate static void {FunctionSignature}()\n" +
                "\t{\n" +
                $"\t\tSceneJumper.ChangeScene(\"{SceneEditorPath}\");\n" +
                "\t}\n";
        }
    }

    [MenuItem("Jumper/Play Game")]
    public static void PlaySceneMenu()
    {
        PlayScene("Assets/Olympus/Scenes/main.unity");
    }

    /// <summary>Extrack scene path data by relative data</summary>
    private static List<ScenePathData> getScenePathData(string targetDirectory, string relativePath)
    {
        List<ScenePathData> scenePathDataList = new List<ScenePathData>();
        Queue<string> sceneDirectories = new Queue<string>(Directory.GetDirectories(targetDirectory));

        while (sceneDirectories.Count != 0)
        {
            string curDirectory = sceneDirectories.Dequeue();
            foreach (var d in Directory.GetDirectories(curDirectory))
            {
                sceneDirectories.Enqueue(d);
            }

            var files = Directory.GetFiles(curDirectory);

            foreach (var filePath in files)
            {
                string curExtension = Path.GetExtension(filePath);

                if (mSceneExtension == curExtension)
                {
                    scenePathDataList.Add(new ScenePathData(filePath, relativePath));
                }
            }
        }

        return scenePathDataList;
    }

    [MenuItem("Jumper/Setup scene quick menu", priority = 300)]
    public static void SetupSceneQuickMenu()
    {
        // Generate scene jump code
        string header = "#if UNITY_EDITOR\n" +
            "using UnityEditor;\n" +
            "\n" +
            "[InitializeOnLoad]\n" +
            "public class SceneJumperExtension : EditorWindow\n" +
            "{\n" +
            "\n";

        string footer = "\n" +
            "}\n" +
            "\n" +
            "#endif";

        string data = header;

        var allScenePath = getScenePathData(mAllSceneAbsolutePath, mAllSceneRelativePath);
        foreach (var sceneData in allScenePath)
        {
            data += sceneData.GetFunctionDeclaration(200);
        }
        data += footer;

        if (FileHandler.TrySaveToFile(mExtensionFilePath, data))
        {
            Debug.Log($"Save file success at {mExtensionFilePath}");
        }
        else
        {
            Debug.Log($"Saveing failed!");
        }

        AssetDatabase.Refresh();
    }

    #region Scene Management Functions

    public static void PlayScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }

        ChangeScene(scenePath);
        EditorApplication.isPlaying = true;
    }

    public static void ChangeScene(string scenePath)
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.ExitPlaymode();
        }

        // If the scene has been modified, Editor ask to you want to save it.
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        EditorSceneManager.OpenScene(scenePath);
    }

    #endregion

    public static string GetThisCodeFilePath([CallerFilePath] string path = null)
    {
        return path;
    }

    public static string GetThisCodeFileDirectory()
    {
        return Path.GetDirectoryName(GetThisCodeFilePath());
    }

    public static string GetThisCodeFileNameWithoutExtension()
    {
        return Path.GetFileNameWithoutExtension(GetThisCodeFilePath());
    }

}

public static class PathExtension
{
    /// <summary>낮은 버전에서 지원하지 않는 문제 해결</summary>
    public static string GetRelativePath(string relativeTo, string path)
    {
        string relativePath = Path.GetFullPath(path).Replace(Path.GetFullPath(relativeTo), "");
        if (relativePath[0] == '/' || relativePath[0] == '\\')
        {
            relativePath = relativePath.Remove(0, 1);
        }
        return relativePath;
    }
}

public static class FileHandler
{
    #region Saving

    public static bool TrySaveToFile(string filePath, string data)
    {
        return TrySaveToFile(new Uri(filePath), data);
    }

    public static bool TrySaveToFile(Uri fileUri, string data)
    {
        try
        {
            string fileDirectory = Path.GetDirectoryName(fileUri.OriginalString);
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(data);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FileHandler] {e}");
            return false;
        }
    }

    public static async Task<bool> TrySaveToFileAsync(Uri fileUri, string data)
    {
        try
        {
            string fileDirectory = Path.GetDirectoryName(fileUri.OriginalString);
            if (!Directory.Exists(fileDirectory))
            {
                Directory.CreateDirectory(fileDirectory);
            }

            using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Create))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                await sw.WriteAsync(data);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FileHandler] {e}");
            return false;
        }
    }

    #endregion

    #region Loading

    public static bool TryLoadTextFromFile(string filePath, out string data)
    {
        return TryLoadTextFromFile(new Uri(filePath), out data);
    }

    public static bool TryLoadTextFromFile(Uri fileUri, out string data)
    {
        try
        {
            if (!File.Exists(fileUri.OriginalString))
            {
                Debug.LogWarning($"[FileHandler] There is no such file exist! file : {fileUri}");
                data = string.Empty;
                return false;
            }

            using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                data = sr.ReadToEnd();
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FileHandler] {e}");
            data = string.Empty;
            return false;
        }
    }

    public static async Task<string> TryLoadTextFromFileAsync(Uri fileUri)
    {
        try
        {
            if (!File.Exists(fileUri.OriginalString))
            {
                Debug.LogWarning($"[FileHandler] There is no such file exist! file : {fileUri}");
                return string.Empty;
            }

            using (FileStream fs = new FileStream(fileUri.OriginalString, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                return await sr.ReadToEndAsync();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[FileHandler] {e}");
            return string.Empty;
        }
    }

    #endregion
}

#endif
