﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FCSCommon.Objects;
using FCSCommon.Utilities;
using Oculus.Newtonsoft.Json;
using QuantumTeleporter.Mono;
using SMLHelper.V2.Utility;
using UnityEngine;

namespace QuantumTeleporter.Configuration
{
    internal static class Mod
    {
        #region Private Members
        private static ModSaver _saveObject;
        private static SaveData _saveData;
        #endregion

        #region Internal Properties
        internal static string ModName => "Quantum Teleporter";
        internal static string ModFolderName => "FCS_QuantumTeleporter";
        internal static string BundleName => "quantumteleportermodbundle";

        internal const string SaveDataFilename = "QuantumTeleporterSaveData.json";

        internal static string MODFOLDERLOCATION => GetModPath();
        internal static string FriendlyName => "Quantum Teleporter";
        internal static string Description => "A teleporter that allows you to teleport from one base to another";
        internal static string ClassID => FCSTechFabricator.Configuration.QuantumTeleporterClassID;

        internal static event Action<SaveData> OnDataLoaded;
        #endregion

        #region Internal Methods
        internal static void Save()
        {
            if (!IsSaving())
            {
                _saveObject = new GameObject().AddComponent<ModSaver>();

                SaveData newSaveData = new SaveData();

                var controllers = GameObject.FindObjectsOfType<QuantumTeleporterController>();

                foreach (var controller in controllers)
                {
                    controller.Save(newSaveData);
                }

                _saveData = newSaveData;

                ModUtils.Save<SaveData>(_saveData, SaveDataFilename, GetSaveFileDirectory(), OnSaveComplete);
            }
        }

        internal static bool IsSaving()
        {
            return _saveObject != null;
        }

        internal static void OnSaveComplete()
        {
            _saveObject.StartCoroutine(SaveCoroutine());
        }

        internal static string GetSaveFileDirectory()
        {
            return Path.Combine(SaveUtils.GetCurrentSaveDataDir(), ModName);
        }

        internal static SaveDataEntry GetSaveData(string id)
        {
            LoadData();

            var saveData = GetSaveData();

            foreach (var entry in saveData.Entries)
            {
                if (entry.ID == id)
                {
                    return entry;
                }
            }

            return new SaveDataEntry() { ID = id };
        }

        internal static SaveData GetSaveData()
        {
            return _saveData ?? new SaveData();
        }

        internal static void LoadData()
        {
            QuickLogger.Info("Loading Save Data...");
            ModUtils.LoadSaveData<SaveData>(SaveDataFilename, GetSaveFileDirectory(), (data) =>
            {
                _saveData = data;
                QuickLogger.Info("Save Data Loaded");
                OnDataLoaded?.Invoke(_saveData);
            });
        }

        internal static string ConfigurationFile()
        {
            return Path.Combine(MODFOLDERLOCATION, "config.json");
        }
        #endregion

        #region Private Methods
        private static IEnumerator SaveCoroutine()
        {
            while (SaveLoadManager.main != null && SaveLoadManager.main.isSaving)
            {
                yield return null;
            }
            GameObject.DestroyImmediate(_saveObject.gameObject);
            _saveObject = null;
        }
        public static string GetAssetFolder()
        {
            return Path.Combine(GetModPath(), "Assets");
        }


        private static string GetModPath()
        {
            return Path.Combine(GetQModsPath(), ModFolderName);
        }
        private static string GetQModsPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "QMods");
        }

        internal static List<SerializableColor> SerializedColors()
        {
            return JsonConvert.DeserializeObject<List<SerializableColor>>(File.ReadAllText(Path.Combine(GetAssetFolder(), "colors.json")));
        }

        #endregion
    }
}