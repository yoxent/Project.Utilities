using NUnit.Framework;
using ProjectUtilities.Localization.Core;
using UnityEditor;
using UnityEngine;

namespace ProjectUtilities.Localization.Editor
{
    /// <summary>
    /// EditMode tests for LocalizationManager: init with two languages, Get, and fallback.
    /// </summary>
    public class LocalizationManagerEditModeTest
    {
        private LocalizationManager _manager;
        private LocalizationConfig _config;
        private LocalizedTextTable _tableEn;
        private LocalizedTextTable _tableEs;

        [SetUp]
        public void SetUp()
        {
            _tableEn = ScriptableObject.CreateInstance<LocalizedTextTable>();
            _tableEs = ScriptableObject.CreateInstance<LocalizedTextTable>();
            SetTableViaSerializedObject(_tableEn, LanguageCode.English, "test_key", "Hello");
            SetTableViaSerializedObject(_tableEs, LanguageCode.Spanish, "test_key", "Hola");

            _config = ScriptableObject.CreateInstance<LocalizationConfig>();
            SetConfigViaSerializedObject(_config, LanguageCode.English, new[] { _tableEn, _tableEs });

            _manager = new LocalizationManager();
            _manager.Initialize(_config);
        }

        [TearDown]
        public void TearDown()
        {
            if (_tableEn != null) Object.DestroyImmediate(_tableEn);
            if (_tableEs != null) Object.DestroyImmediate(_tableEs);
            if (_config != null) Object.DestroyImmediate(_config);
        }

        [Test]
        public void Initialize_WithTwoLanguages_Get_ReturnsDefaultLanguageValue()
        {
            Assert.That(_manager.Get("test_key"), Is.EqualTo("Hello"));
        }

        [Test]
        public void SetLanguage_ToSpanish_Get_ReturnsSpanishValue()
        {
            _manager.SetLanguage(LanguageCode.Spanish);
            Assert.That(_manager.Get("test_key"), Is.EqualTo("Hola"));
        }

        [Test]
        public void Get_MissingKey_ReturnsKeyAsFallback()
        {
            Assert.That(_manager.Get("missing_key"), Is.EqualTo("missing_key"));
        }

        [Test]
        public void TryGet_ExistingKey_ReturnsTrueAndValue()
        {
            var found = _manager.TryGet("test_key", out var value);
            Assert.That(found, Is.True);
            Assert.That(value, Is.EqualTo("Hello"));
        }

        [Test]
        public void TryGet_MissingKey_ReturnsFalse()
        {
            var found = _manager.TryGet("missing_key", out var value);
            Assert.That(found, Is.False);
            Assert.That(value, Is.Null);
        }

        private static void SetTableViaSerializedObject(LocalizedTextTable table, LanguageCode language, string key, string value)
        {
            var so = new SerializedObject(table);
            so.FindProperty("_language").intValue = (int)language;
            var entries = so.FindProperty("_entries");
            entries.ClearArray();
            entries.InsertArrayElementAtIndex(0);
            var element = entries.GetArrayElementAtIndex(0);
            element.FindPropertyRelative("Key").stringValue = key;
            element.FindPropertyRelative("Value").stringValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetConfigViaSerializedObject(LocalizationConfig config, LanguageCode defaultLanguage, LocalizedTextTable[] tables)
        {
            var so = new SerializedObject(config);
            so.FindProperty("_defaultLanguage").intValue = (int)defaultLanguage;
            var list = so.FindProperty("_tables");
            list.ClearArray();
            for (int i = 0; i < tables.Length; i++)
            {
                list.InsertArrayElementAtIndex(i);
                list.GetArrayElementAtIndex(i).objectReferenceValue = tables[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
