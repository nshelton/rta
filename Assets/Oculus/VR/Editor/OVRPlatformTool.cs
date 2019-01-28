using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Diagnostics;
using UnityEngine.Networking;
using System.Net;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace Assets.Oculus.VR.Editor
{
	public class OVRPlatformTool : EditorWindow
	{
		enum TargetPlatform
		{
			Rift,
			OculusGoGearVR,
			Quest,
		};
	
		const string urlPlatformUtil =
			"https://www.oculus.com/download_app/?id=1076686279105243";

		static private TargetPlatform mTargetPlatform;
		Vector2 scroll;

		static public string log;

		[MenuItem("Oculus/Tools/Oculus Platform Tool")]
		static void Init()
		{
			OVRPlatformTool.log = String.Empty;
			// Get existing open window or if none, make a new one:
			EditorWindow.GetWindow(typeof(OVRPlatformTool));
#if UNITY_ANDROID
			mTargetPlatform = TargetPlatform.OculusGoGearVR;
#else
			mTargetPlatform = TargetPlatform.Rift;
#endif
			OVRPlugin.SendEvent("oculus_platform_tool", "show_window");
		}

		void OnGUI()
		{
			GUILayout.Label("OVR Platform Tool", EditorStyles.boldLabel);
			this.titleContent.text = "OVR Platform Tool";
			string[] options = new string[]
			{
				"Oculus Rift",
				"Oculus Go | Gear VR",
				"Oculus Quest"
			};
			mTargetPlatform = (TargetPlatform) EditorGUILayout.Popup("Target Oculus Platform", (int)mTargetPlatform, options);

			GUILayout.BeginVertical(GUILayout.Height(Screen.height/2));
			{
				// Add the UI Form
				GUILayout.FlexibleSpace();
		
				// App ID
				GUIContent AppIDLabel = new GUIContent("Oculus Application ID [?]: ",
					"This AppID will be used when uploading the build.");
				OVRPlatformToolSettings.AppID = MakeTextBox(AppIDLabel, OVRPlatformToolSettings.AppID);

				// App Token
				GUIContent AppTokenLabel = new GUIContent("Oculus App Token [?]: ",
					"You can get your app token from your app's Oculus API Dashboard.");
				OVRPlatformToolSettings.AppToken = MakePasswordBox(AppTokenLabel, OVRPlatformToolSettings.AppToken);

				// Release Channel
				GUIContent ReleaesChannelLabel = new GUIContent("Release Channel [?]: ",
					"Specify the releaes channel of the new build, you can reassign to other channels after upload.");
				OVRPlatformToolSettings.ReleaesChannel = MakeTextBox(ReleaesChannelLabel, OVRPlatformToolSettings.ReleaesChannel);

				// Releaes Note
				GUIContent ReleaseNoteLabel = new GUIContent("Release Note: ");
				OVRPlatformToolSettings.ReleaseNote = MakeTextBox(ReleaseNoteLabel, OVRPlatformToolSettings.ReleaseNote);

				// Platform specific fields
				if (mTargetPlatform == TargetPlatform.Rift)
				{
					GUIContent BuildDirLabel = new GUIContent("Rift Build Directory [?]: ",
						"The full path to the directory containing your Rift build files.");
					OVRPlatformToolSettings.RiftBuildDirectory = MakeTextBox(BuildDirLabel, OVRPlatformToolSettings.RiftBuildDirectory);

					GUIContent BuildVersionLabel = new GUIContent("Build Version [?]: ",
						"The version number shown to users.");
					OVRPlatformToolSettings.RiftBuildVersion = MakeTextBox(BuildVersionLabel, OVRPlatformToolSettings.RiftBuildVersion);

					GUIContent LaunchFileLabel = new GUIContent("Launch File Path [?]: ",
						"The relative path from <BuildPath> to the executable that launches your app.");
					OVRPlatformToolSettings.RiftLaunchFile = MakeTextBox(LaunchFileLabel, OVRPlatformToolSettings.RiftLaunchFile);
				}
				else
				{
					GUIContent ApkPathLabel = new GUIContent("Build APK File Path [?]: ",
						"The full path to the APK file.");
					OVRPlatformToolSettings.ApkBuildPath = MakeTextBox(ApkPathLabel, OVRPlatformToolSettings.ApkBuildPath);
				}

				GUILayout.FlexibleSpace();
				// Add an Upload buttom
				GUIContent btnTxt = new GUIContent("Upload");
				var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, GUILayout.ExpandWidth(false));
				rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
				if (GUI.Button(rt, btnTxt, GUI.skin.button))
				{
					OVRPlugin.SendEvent("oculus_platform_tool", "upload");
					OVRPlatformTool.log = String.Empty;
					OnUpload(mTargetPlatform);
				}

				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();

			scroll = EditorGUILayout.BeginScrollView(scroll);
			EditorGUILayout.LabelField(OVRPlatformTool.log, GUILayout.Height(position.height - 30));
			EditorGUILayout.EndScrollView();
		}

		private void OnUpload(TargetPlatform targetPlatform)
		{
			OVRPlatformTool.log = String.Empty;
			SetDirtyOnGUIChange();
			ExecuteCommand(targetPlatform);
		}

	  static void ExecuteCommand(TargetPlatform targetPlatform)
		{
			string dataPath = Application.dataPath.ToString();
			var thread = new Thread(delegate () {
				Command(targetPlatform, dataPath);
			});
			thread.Start();
		}

		static void Command(TargetPlatform targetPlatform, string dataPath)
		{
			string toolDataPath = dataPath + "/Oculus/VR/Editor/Tools";
			if (!Directory.Exists(toolDataPath))
			{
				Directory.CreateDirectory(toolDataPath);
			}

			string platformUtil = toolDataPath + "/ovr-platform-util.exe";
			if (!System.IO.File.Exists(platformUtil))
			{
				OVRPlugin.SendEvent("oculus_platform_tool", "provision_util");
				EditorCoroutine downloadCoroutine = EditorCoroutine.Start(ProvisionPlatformUtil(platformUtil));
				while (!downloadCoroutine.GetCompleted()) { }
			}

			string args = genUploadCommand(targetPlatform);

			var process = new Process();
			var processInfo = new ProcessStartInfo(platformUtil, args);

			processInfo.CreateNoWindow = true;
			processInfo.UseShellExecute = false;
			processInfo.RedirectStandardError = true;
			processInfo.RedirectStandardOutput = true;

			process.StartInfo = processInfo;

			process.OutputDataReceived += new DataReceivedEventHandler(
				(s, e) =>
				{
					if (e.Data.Length != 0 && !e.Data.Contains("\u001b"))
					{
						OVRPlatformTool.log += e.Data + "\n";
					}
				}
			);
			process.ErrorDataReceived += new DataReceivedEventHandler(
				(s, e) =>
				{
					OVRPlatformTool.log += e.Data + "\n";
				}
			);

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
		}

		private static string genUploadCommand(TargetPlatform targetPlatform)
		{
			string command = "";
			switch (targetPlatform)
			{
				case TargetPlatform.Rift:
					command = "upload-rift-build";
					break;
				case TargetPlatform.OculusGoGearVR:
					command = "upload-mobile-build";
					break;
				case TargetPlatform.Quest:
					command = "upload-quest-build";
					break;
				default:
					return command;
			}

			// Add App ID
			command += " --app-id " + OVRPlatformToolSettings.AppID;

			// Add App Token
			command += " --app-secret " + OVRPlatformToolSettings.AppToken;

			// Add Platform specific fields
			if (targetPlatform == TargetPlatform.Rift)
			{
				command += " --build-dir " + OVRPlatformToolSettings.RiftBuildDirectory;
				command += " --launch-file " + OVRPlatformToolSettings.RiftLaunchFile ;
				command += " --version " + OVRPlatformToolSettings.RiftBuildVersion;
			}
			else
			{
				command += " --apk " + OVRPlatformToolSettings.ApkBuildPath;
			}

			// Add Channel
			command += " --channel " + OVRPlatformToolSettings.ReleaesChannel;

			// Add Notes
			command += " --notes " + OVRPlatformToolSettings.ReleaseNote;

			return command;
		}

		void OnInspectorUpdate()
		{
			Repaint();
		}

		private string MakeTextBox(GUIContent label, string variable)
		{
			return GUIHelper.MakeControlWithLabel(label, () => {
				GUI.changed = false;
				var result = EditorGUILayout.TextField(variable);
				SetDirtyOnGUIChange();
				return result;
			});
		}

		private string MakePasswordBox(GUIContent label, string variable)
		{
			return GUIHelper.MakeControlWithLabel(label, () => {
				GUI.changed = false;
				var result = EditorGUILayout.PasswordField(variable);
				SetDirtyOnGUIChange();
				return result;
			});
		}

		private static void SetDirtyOnGUIChange()
		{
			if (GUI.changed)
			{
				EditorUtility.SetDirty(OVRPlatformToolSettings.Instance);
				GUI.changed = false;
			}
		}

		private static IEnumerator ProvisionPlatformUtil(string dataPath)
		{
			using (WWW www = new WWW(urlPlatformUtil))
			{
				UnityEngine.Debug.Log("Started Provisioning Oculus Platform Util");
				float timer = 0;
				float timeOut = 60;
				yield return www;
				while (!www.isDone && timer < timeOut)
				{
					timer += Time.deltaTime;
					if (www.error != null)
					{
						UnityEngine.Debug.Log("Download error: " + www.error);
						break;
					}
					OVRPlatformTool.log = string.Format("Downloading.. {0:P1}", www.progress);
					SetDirtyOnGUIChange();
					yield return new WaitForSeconds(1f);
				}
				if (www.isDone)
				{
					System.IO.File.WriteAllBytes(dataPath, www.bytes);
					OVRPlatformTool.log = "Completed Provisioning Oculus Platform Util";
					SetDirtyOnGUIChange();
				}
			}
		}

		class GUIHelper
		{
			public delegate void Worker();

			static void InOut(Worker begin, Worker body, Worker end)
			{
				try
				{
					begin();
					body();
				}
				finally
				{
					end();
				}
			}

			public static void HInset(int pixels, Worker worker)
			{
				InOut(
					() => {
						GUILayout.BeginHorizontal();
						GUILayout.Space(pixels);
						GUILayout.BeginVertical();
					},
					worker,
					() => {
						GUILayout.EndVertical();
						GUILayout.EndHorizontal();
					}
				);
			}

			public delegate T ControlWorker<T>();
			public static T MakeControlWithLabel<T>(GUIContent label, ControlWorker<T> worker)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(label);

				var result = worker();

				EditorGUILayout.EndHorizontal();
				return result;
			}
		}

		public class EditorCoroutine
		{
			public static EditorCoroutine Start(IEnumerator routine)
			{
				EditorCoroutine coroutine = new EditorCoroutine(routine);
				coroutine.Start();
				return coroutine;
			}

			readonly IEnumerator routine;
		  bool completed;
			EditorCoroutine(IEnumerator _routine)
			{
				routine = _routine;
				completed = false;
			}

			void Start()
			{
				EditorApplication.update += Update;
			}
			public void Stop()
			{
				EditorApplication.update -= Update;
				completed = true;
			}

			public bool GetCompleted()
			{
				return completed;
			}

			void Update()
			{
				if (!routine.MoveNext())
				{
					Stop();
				}
			}
		}

#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoad]
#endif
		public sealed class OVRPlatformToolSettings : ScriptableObject
		{
			public static string AppID
			{
				get { return Instance.appID; }
				set { Instance.appID = value; }
			}

			public static string AppToken
			{
				get { return Instance.appToken; }
				set { Instance.appToken = value; }
			}

			public static string ReleaseNote
			{
				get { return Instance.releaseNote; }
				set { Instance.releaseNote = value; }
			}

			public static string ReleaesChannel
			{
				get { return Instance.releaseChannel; }
				set { Instance.releaseChannel = value; }
			}

			public static string RiftBuildDirectory
			{
				get { return Instance.riftBuildDiretory; }
				set { Instance.riftBuildDiretory = value; }
			}

			public static string ApkBuildPath
			{
				get { return Instance.apkBuildPath; }
				set { Instance.apkBuildPath = value; }
			}

			public static string RiftBuildVersion
			{
				get { return Instance.riftBuildVersion; }
				set { Instance.riftBuildVersion = value; }
			}

			public static string RiftLaunchFile
			{
				get { return Instance.riftLaunchFile; }
				set { Instance.riftLaunchFile = value; }
			}

			[SerializeField]
			private string appID = "";

			[SerializeField]
			private string appToken = "";

			[SerializeField]
			private string releaseNote = "";

			[SerializeField]
			private string releaseChannel = "Beta";

			[SerializeField]
			private string riftBuildDiretory = "";

			[SerializeField]
			private string riftBuildVersion = "";

			[SerializeField]
			private string riftLaunchFile = "";

			[SerializeField]
			private string apkBuildPath = "";

			private static OVRPlatformToolSettings instance;
			public static OVRPlatformToolSettings Instance
			{
				get
				{
					if (instance == null)
					{
						instance = Resources.Load<OVRPlatformToolSettings>("OVRPlatformToolSettings");

						if (instance == null)
						{
							instance = ScriptableObject.CreateInstance<OVRPlatformToolSettings>();

							string properPath = System.IO.Path.Combine(UnityEngine.Application.dataPath, "Resources");
							if (!System.IO.Directory.Exists(properPath))
							{
								UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
							}

							string fullPath = System.IO.Path.Combine(
								System.IO.Path.Combine("Assets", "Resources"),
								"OVRPlatformToolSettings.asset"
							);
							UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);

						}
					}
					return instance;
				}
				set
				{
					instance = value;
				}
			}
		}

	}
}
