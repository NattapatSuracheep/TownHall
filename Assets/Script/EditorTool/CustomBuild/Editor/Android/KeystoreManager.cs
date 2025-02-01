using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class KeystoreManager : BuildPlatformMenu
{
    public class KeystoreConfig
    {
        public string path;
        public string name;
        public string password;
        public string alias;
        public string aliasPassword;
    }

    private static KeystoreConfig KeystoreData => CustomBuildPipeline.Config.keystore;

    public static void KeystoreInfo()
    {
        GUILayout.Label("Keystore Info", EditorStyles.boldLabel);
        GUILayout.Space(5);

        if (IsKeystoreExist())
        {
            GUILayout.Label("Keystore Path: ");
            GUILayout.Label("\t" + KeystoreData.path, WarningStyle);
            GUILayout.Space(5);

            GUILayout.Label("Keystore Name: ");
            GUILayout.Label("\t" + KeystoreData.name, WarningStyle);
            GUILayout.Space(5);

            GUILayout.Label("Keystore Password: ");
            GUILayout.Label("\t" + KeystoreData.password, WarningStyle);
            GUILayout.Space(5);

            GUILayout.Label("Keystore Alias: ");
            GUILayout.Label("\t" + KeystoreData.alias, WarningStyle);
            GUILayout.Space(5);

            GUILayout.Label("Keystore Alias Password: ");
            GUILayout.Label("\t" + KeystoreData.aliasPassword, WarningStyle);
            GUILayout.Space(5);
        }
        else
        {
            GUILayout.Label($"Keystore file not found on path: {KeystoreData.path}", NotMatchStyle);
        }
    }

    private static bool IsKeystoreExist()
    {
        return File.Exists(KeystoreData.path);
    }

    public static void UpdateKeystore()
    {
        if (IsKeystoreExist() == false)
        {
            Debug.LogError($"Can't update keystore, Keystore file not found: {KeystoreData.path}");
            return;
        }

        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = KeystoreData.path;
        PlayerSettings.Android.keyaliasName = KeystoreData.alias;
        PlayerSettings.Android.keyaliasPass = KeystoreData.password;
        PlayerSettings.Android.keystorePass = KeystoreData.aliasPassword;
    }
}