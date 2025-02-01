#if UNITY_IOS || UNITY_IPHONE

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.IO;
using System.Linq;

///<summary>
/// 1. To set localization to TH and EN only
/// 2. To automatically set pod, capabilities, bitcode, etc for xcode
/// 3. To add ask ATT permission at start of application
///</summary>
public static class iOSPostProcessBuild
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            SetMainXcodeProject();
        }

        void SetMainXcodeProject()
        {
            //Set values in project.pbxproj
            var projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            var mainTarget = pbxProject.GetUnityMainTargetGuid();
            pbxProject.SetBuildProperty(mainTarget, "ENABLE_BITCODE", "NO"); // Disable bitcode for every target except for Pods
            pbxProject.AddFrameworkToProject(mainTarget, "GameKit.framework", false); // Add gamekit to framework
            pbxProject.AddFrameworkToProject(mainTarget, "UserNotifications.framework", false); // Add UserNotifications to framework
            pbxProject.AddFrameworkToProject(mainTarget, "AppTrackingTransparency.framework", true); // Add AppTrackingTransparency to framework
            SetCapabilities(projectPath, mainTarget);


            var frameworkTarget = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(frameworkTarget, "ENABLE_BITCODE", "NO");
            pbxProject.AddBuildProperty(frameworkTarget, "OTHER_LDFLAGS", "-ld64");
            pbxProject.SetBuildProperty(frameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO"); // Fix error framework disallowed

            pbxProject.WriteToFile(projectPath); //Save changes

            SetLocalization(projectPath); //Call at the end because it is set manually, not by Unity API
        }

        void SetCapabilities(string projectPath, string target)
        {
            ProjectCapabilityManager manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", targetGuid: target);
#if SERVER_PRODUCTION || SERVER_STAGING
            var isDevelopmentMode = false;
#else
            var isDevelopmentMode = true;
#endif
            // manager.AddPushNotifications(isDevelopmentMode); //add push noti
            manager.WriteToFile();
        }

        void SetLocalization(string projectPath)
        {
            var text = File.ReadAllLines(projectPath);
            var searchingWord = new string[] { "developmentRegion", "knownRegions" };
            var step = 0;
            for (var i = 0; i < text.Length && step < searchingWord.Length; i++)
            {
                var line = text[i];
                var foundIndex = line.IndexOf(searchingWord[step]);
                if (foundIndex >= 0)
                {
                    Process(foundIndex, ref text, ref i);
                }
            }

            File.WriteAllLines(projectPath, text);

            void Process(int foundIndex, ref string[] text, ref int lineIndex)
            {
                switch (step)
                {
                    case 0:
                        text[lineIndex] = text[lineIndex].Replace("English", "th");
                        break;
                    case 1:
                        for (var i = lineIndex; i < text.Length; i++)
                        {
                            if (text[i].IndexOf(");") >= 0)
                            {
                                text[i] = "knownRegions = (\"th\", \"en\");";
                                break;
                            }

                            text[i] = string.Empty;
                        }
                        break;
                }
                UnityEngine.Debug.Log($"Done set localize step: {step}");
                step++;
            }
        }

        UnityEngine.Debug.Log("done edit xcode project file");
    }

    [PostProcessBuild(999)]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {
        // Get plist
        string plistPath = pathToBuiltProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        // Get root
        PlistElementDict rootDict = plist.root;

        //Set skip 'Missing Export Compliance Information'
        rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

        var array = rootDict["CFBundleURLTypes"].AsArray();
        var dict = array.AddDict();
        dict.SetString("CFBundleURLName", PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));
        dict.CreateArray("CFBundleURLSchemes").AddString(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));

        plist.root = rootDict;

        var plistString = plist.WriteToString();

        File.WriteAllText(plistPath, plistString);
        UnityEngine.Debug.Log("done edit info plist");
    }
}

#endif