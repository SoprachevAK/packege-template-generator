using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.IO;
using System.Linq;
using UnityEditor.Compilation;

namespace AS.PackegeTemplateGenerator
{

    public class PackegeTemplateGenerator : EditorWindow
    {
        struct Author
        {
            public string name;
            public string email;
            public string url;
        }

        const string BANDLE_NAME_KEY = "PackegeTemplateGenerator/bandleName";
        const string AUTHOR_KEY = "PackegeTemplateGenerator/Author";

        string bandleName = "com.author.name";
        string version = "1.0.0";
        string displayName = "name";
        string description = "description";
        string gitURL = "https://github.com/";
        string usage = "";
        public List<string> tags = new List<string>();

        bool runtime = true;
        bool editor = true;


        Author author;


        [MenuItem("AS/Tools/PackegeTemplateGenerator")]
        static void Init()
        {

            PackegeTemplateGenerator window = (PackegeTemplateGenerator)EditorWindow.GetWindow(typeof(PackegeTemplateGenerator));
            window.Show();

            if (PlayerPrefs.HasKey(BANDLE_NAME_KEY))
            {
                window.bandleName = PlayerPrefs.GetString(BANDLE_NAME_KEY);
            }

            if (PlayerPrefs.HasKey(AUTHOR_KEY))
            {
                window.author = JsonUtility.FromJson<Author>(PlayerPrefs.GetString(AUTHOR_KEY));
            }
        }

        void OnGUI()
        {
            DrawBase();
            DrawAuthor();
            GUILayout.Space(20);
            DrawGenerated();
            DrawUsage();



            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create"))
            {
                Create();
            }
            GUILayout.Space(20);
        }

        void DrawBase()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            bandleName = EditorGUILayout.TextField("packege name", bandleName);
            version = EditorGUILayout.TextField("version", version);
            displayName = EditorGUILayout.TextField("displayName", displayName);
            description = EditorGUILayout.TextField("description", description);
            EditorGUI.indentLevel = 0;
        }


        void DrawAuthor()
        {
            GUILayout.Label("Author", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            author.name = EditorGUILayout.TextField("name", author.name);
            author.email = EditorGUILayout.TextField("email", author.email);
            author.url = EditorGUILayout.TextField("url", author.url);
            EditorGUI.indentLevel = 0;
        }

        void DrawGenerated()
        {
            GUILayout.Label("Generated", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            gitURL = EditorGUILayout.TextField("git URL", gitURL);
            runtime = EditorGUILayout.Toggle("runtime", runtime);
            editor = EditorGUILayout.Toggle("editor", editor);
            EditorGUI.indentLevel = 0;
        }

        void DrawUsage()
        {
            GUILayout.Label("Readme", EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("Usage:");
            usage = EditorGUILayout.TextArea(usage);
            EditorGUI.indentLevel = 0;
        }



        void Create()
        {
            string project = Path.GetDirectoryName(Application.dataPath);
            string packeges = Path.Combine(project, "Packages");
            if (!Directory.Exists(packeges))
            {
                Directory.CreateDirectory(packeges);
            }

            string packege = Path.Combine(packeges, bandleName);
            Directory.CreateDirectory(packege);

            if (runtime)
            {
                string asmdefRuntime = Resources.Load("PackegeTemplateGenerator/asmdefRuntime").ToString();
                asmdefRuntime = asmdefRuntime
                    .Replace("{{NAME}}", bandleName + ".Runtime");

                string runtimePath = Path.Combine(packege, "Runtime");
                Directory.CreateDirectory(runtimePath);
                File.WriteAllText(Path.Combine(runtimePath, $"{bandleName}.Runtime.asmdef"), asmdefRuntime);

            }

            if (editor)
            {
                string asmdefEditor = Resources.Load("PackegeTemplateGenerator/asmdefEditor").ToString().ToString();
                asmdefEditor = asmdefEditor
                    .Replace("{{NAME}}", bandleName + ".Editor");

                string editorPath = Path.Combine(packege, "Editor");
                Directory.CreateDirectory(editorPath);
                File.WriteAllText(Path.Combine(editorPath, $"{bandleName}.Editor.asmdef"), asmdefEditor);
            }

            string README = Resources.Load("PackegeTemplateGenerator/README").ToString();
            string CHANGELOG = Resources.Load("PackegeTemplateGenerator/CHANGELOG").ToString();
            string packageJson = Resources.Load("PackegeTemplateGenerator/package").ToString();



            README = README.
                Replace("{{GITURL}}", gitURL + ".git").
                Replace("{{AUTHOR}}", author.name).
                Replace("{{USAGE}}", usage);

            CHANGELOG = CHANGELOG.
                Replace("{{DATE}}", System.DateTime.Now.ToString("yyyy-MM-dd"));

            packageJson = packageJson.
                Replace("{{BANDLE_NAME}}", bandleName).
                Replace("{{VERSION}}", version).
                Replace("{{DISPLAY_NAME}}", displayName).
                Replace("{{DESCRIPTION}}", description).
                Replace("{{KEYWORDS}}", string.Join(",\n", displayName.Split(' ').Select(t => $"\"{t}\""))).
                Replace("{{AUTHOR_NAME}}", author.name).
                Replace("{{AUTHOR_EMAIL}}", author.email).
                Replace("{{AUTHOR_URL}}", author.url);

            File.WriteAllText(Path.Combine(packege, "README.md"), README);
            File.WriteAllText(Path.Combine(packege, "CHANGELOG.md"), CHANGELOG);
            File.WriteAllText(Path.Combine(packege, "package.json"), packageJson);
            File.WriteAllText(Path.Combine(packege, ".gitignore"), Resources.Load("PackegeTemplateGenerator/gitignore").ToString());


            PlayerPrefs.SetString(BANDLE_NAME_KEY, bandleName);
            PlayerPrefs.SetString(AUTHOR_KEY, JsonUtility.ToJson(author));

            EditorUtility.DisplayDialog("PackegeTemplateGenerator", "Packege generated successfully, refresh unity", "Ok", null);
        }
    }


}