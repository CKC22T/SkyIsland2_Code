using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Olympus
{
    public class AutoEnumerationGenerator<T> : EditorWindow where T : AutoEnumerationGenerator<T>
    {
        protected string beginAnchorString = "// <<";
        protected string endAnchorString = "// >>";

        protected string target;
        protected string trackedSourceFile;
        protected string filter;

        protected List<string> assetList = new List<string>();
        protected Dictionary<string, string> exceptions;

        static Dictionary<int, Dictionary<string, string>> globalExceptionList
            = new Dictionary<int, Dictionary<string, string>>();

        static protected string exceptionFilePath = "Assets/Olympus/Scripts/Addons/Exceptions.txt";

        private string exceptionKey = "";
        private string exceptionValue = "";

        static readonly char[] specialCases
            = { '!', '@', '#', '$',
                '%', '^', '&', '*',
                '(', ')', '-', '+',
                '=', '/', '`', '~',
                '.', ',', '?', ';',
                ':', '\'', '"','}',
                '{', '[', ']', '|',
                '\\' };

        static readonly char[] numericCases =
        {
            '0', '1', '2', '3', '4',
            '5', '6', '7', '8', '9'
        };

        string sourceBuffer;

        static AutoEnumerationGenerator()
        {
            EditorApplication.projectChanged += ProjectChangeCallBack;
        }

        private void OnDestroy()
        {
            WriteEnumerationToFile();
        }

        void WriteEnumerationToFile()  
        {
            StreamWriter fileWriter;
            if (File.Exists(exceptionFilePath) == false)
            {
                fileWriter = File.CreateText(exceptionFilePath);
                fileWriter.Close();
            }

            StreamReader fileReader = new StreamReader(exceptionFilePath);

            string buffer = fileReader.ReadToEnd();
            fileReader.Close();

            fileWriter = new StreamWriter(exceptionFilePath);
            string className = (Instance as T).GetType().Name;

            if(buffer.Contains(className) == true)
            {
                int headerSize = className.Length + 1;
                int headerIndex = buffer.IndexOf(className);

                int endIndex = buffer.IndexOf('$', headerIndex);

                int contentIndex = headerSize + headerIndex;
                
                // clearing contents
                int contentSize = endIndex - contentIndex;
                buffer = buffer.Remove(contentIndex, contentSize);

                foreach (var p in exceptions)
                {
                    string format = p.Key + " > " + p.Value + "\n";

                    buffer = buffer.Insert(contentIndex, format);
                }
            }
            else
            {
                buffer += "\n#" + className + "\n$";

                int headerSize = className.Length + 1;
                int headerIndex = buffer.IndexOf(className);

                int contentIndex = headerSize + headerIndex;

                if (exceptions != null)
                {
                    foreach (var p in exceptions)
                    {
                        string format = p.Key + " > " + p.Value + "\n";

                        buffer = buffer.Insert(contentIndex, format);
                    }
                }
            }

            fileWriter.Write(buffer);
            fileWriter.Close();
        }

        void ReadEnumerationFromFile()
        {
            if (File.Exists(exceptionFilePath) == false)
            {
                StreamWriter writer = File.CreateText(exceptionFilePath);
                writer.Close();
            }

            StreamReader fileReader = new StreamReader(exceptionFilePath);
            string buffer = fileReader.ReadToEnd();
            fileReader.Close();

            StringReader bufferReader = new StringReader(buffer);
            string className = (Instance as T).GetType().Name;

            string line;
            while ((line = bufferReader.ReadLine()) != null)
            {
                if(line == string.Empty || line == null)
                {
                    continue;
                }
                if(line.Contains(className) == true)
                {
                    while(true)
                    {
                        string content = bufferReader.ReadLine();

                        if (content.Contains('$') == true)
                        {
                            break;
                        }

                        int mid = content.IndexOf('>');
                        string key = content.Substring(0, mid - 1);
                        string value = content.Substring(mid + 2);

                        if (exceptions.ContainsKey(key) == false)
                        {
                            exceptions.Add(key, value);
                        }


                    }
                }
                else
                {
                    continue;
                }
            }
        }

        protected void OnEnable()
        {
            int classHash = (Instance as T).GetType().Name.GetHashCode();

            if (globalExceptionList.ContainsKey(classHash) == true)
            {
                Instance.exceptions = globalExceptionList.GetValueOrDefault(classHash);
            }
            else
            {
                Instance.exceptions = new Dictionary<string, string>();
                globalExceptionList.Add(classHash, Instance.exceptions);
            }

            ReadEnumerationFromFile();

        }

        private void OnGUI()
        {
            Rect mainRect = EditorGUILayout.GetControlRect();

            if (GUI.Button(mainRect, "Refresh List"))
            {
                (Instance as T).RefreshAssetList();
            }

            Rect exceptionAddRect = mainRect;
            exceptionAddRect.y += 60;

            exceptionKey = EditorGUILayout.TextArea(exceptionKey);
            exceptionValue = EditorGUILayout.TextArea(exceptionValue);

            if (GUI.Button(exceptionAddRect, "Add Exception"))
            {
                if(exceptionKey == string.Empty || exceptionValue == string.Empty)
                {
                    EditorUtility.DisplayDialog("경고", "잘못된 형식의 인자입니다.", "확인");
                }
                else if (exceptions.ContainsKey(exceptionKey) == true)
                {
                    if(EditorUtility.DisplayDialog("경고", "해당 Key가 이미 존재합니다 할당된 Key에 대한 Value를 변경하시겠습니까? 연결된 Value: " + exceptions.GetValueOrDefault(exceptionKey), "예", "아니오"))
                    {
                        exceptions.Remove(exceptionKey);
                        exceptions.Add(exceptionKey, exceptionValue);
                    }
                }
                else
                {
                    exceptions.Add(exceptionKey, exceptionValue);
                }
            }

            Rect exceptionListLabelRect = exceptionAddRect;
            exceptionListLabelRect.y += 20;

            EditorGUI.LabelField(exceptionListLabelRect, "Exception List");

            Rect keyRect = exceptionListLabelRect;
            Rect valueRect = exceptionListLabelRect;
            keyRect.x += 10;
            valueRect.x += 60;
            foreach (var p in exceptions)
            {
                keyRect.y += 20;
                valueRect.y += 20;
                Rect horizontalRect = EditorGUILayout.BeginHorizontal();

                EditorGUI.LabelField(keyRect, p.Key);
                EditorGUI.LabelField(valueRect, p.Value);
                EditorGUILayout.EndHorizontal();
            }

            keyRect.y += 20;

            Rect assetListRect = keyRect;

            assetListRect.x -= 10;

            EditorGUI.LabelField(assetListRect, "Asset List");

            Rect assetRect = assetListRect;
            assetRect.x += 10;
            assetRect.y += 20;
            foreach (var i in assetList)
            {
                EditorGUI.LabelField(assetRect, i);
                assetRect.y += 20;
            }

        }
        protected static AutoEnumerationGenerator<T> Instance
        {
            get { return GetWindow<AutoEnumerationGenerator<T>>(); }
        }

        static void ProjectChangeCallBack()
        {
            GetCertainTypeAssetsInProject(Instance.filter);
        }

        protected void RefreshAssetList()
        {
            GetCertainTypeAssetsInProject(Instance.filter);
        }

        static string[] GetCertainTypeAssetsInProject(string filter)
        {
            if (Directory.Exists(Instance.target) == false)
            {
                EditorUtility.DisplayDialog("경고", "현재 소스코드에 지정된 경로가 존재하지 않습니다.\n" + Instance.target, "확인");
                return null;
            }

            string[] targets = { Instance.target };
            List<string> assetList = new List<string>();
            assetList.AddRange(AssetDatabase.FindAssets(filter, targets));
            string[] assetPathes = new string[assetList.Count];
            ClearContents(Instance.trackedSourceFile);

            int index = 0;
            foreach (var i in assetList)
            {
                assetPathes[index] = AssetDatabase.GUIDToAssetPath(i);
                string path = assetPathes[index];

                string content = path.Substring(path.LastIndexOf('/') + 1);
                content = content.Substring(0, content.LastIndexOf('.'));

                while (content.Contains(" "))
                {
                    content = content.Replace(" ", "");
                }

                if (Instance.exceptions.ContainsKey(content) == true)
                {
                    content = Instance.exceptions.GetValueOrDefault(content);
                }

                bool IsInvalidName = false;
                foreach (var c in content)
                {
                    for (int j = 0; j < specialCases.Length; j++)
                    {
                        if (c == (char)specialCases.GetValue(j))
                        {
                            IsInvalidName = true;
                            content = content.Replace(c, '\0');
                        }
                    }
                }

                for (int j = 0; j < numericCases.Length; j++)
                {
                    if (content.StartsWith(numericCases[j]))
                    {
                        IsInvalidName = true;
                    }
                }

                if (IsInvalidName == true)
                {
                    if (EditorUtility.DisplayDialog("경고!", "파일 이름을 잘못 지으신거 같은데, 정말 진행하실건가요?", "아니 뭐야 지워줘요", "네"))
                    {
                        AssetDatabase.DeleteAsset(path);
                        continue;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("안내", "소스코드가 고장났어요, 파일을 지우거나 이름을 바꿔도 증상이 지속된다면 'Window -> Auto-Enumeration Geneartor' 창으로 이동하여 'Refresh List' 버튼을 눌러주세요.", "넹");
                    }
                }

                InjectContent(content, Instance.trackedSourceFile);
                Instance.assetList.Add(content);
                index++;
            }

            return assetList.ToArray();
        }
        static void ClearContents(string target)
        {
            StreamReader sourceReader = new StreamReader(target);

            string source = sourceReader.ReadToEnd();
            Instance.sourceBuffer = source;
            sourceReader.Close();

            int beginAnchorIndex = Instance.sourceBuffer.IndexOf(Instance.beginAnchorString);

            int begin = beginAnchorIndex + Instance.beginAnchorString.Length;
            int end = Instance.sourceBuffer.IndexOf(Instance.endAnchorString);

            Instance.sourceBuffer = Instance.sourceBuffer.Remove(begin, end - begin - 1);

            Instance.assetList.Clear();
        }

        static void InjectContent(string content, string target)
        {
            int cursor;

            StreamWriter sourceWriter = new StreamWriter(target);
            cursor = Instance.sourceBuffer.LastIndexOf(Instance.beginAnchorString);

            Instance.sourceBuffer = Instance.sourceBuffer.Insert(cursor + Instance.beginAnchorString.Length, "\n    " + content + ",");
            sourceWriter.Write(Instance.sourceBuffer);
            sourceWriter.Flush();

            sourceWriter.Close();
        }

    }
}

#endif