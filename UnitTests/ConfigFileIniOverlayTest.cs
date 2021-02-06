using ActLikeAI.Config.Ini;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigFileIniOverlayTest
    {
        private ConfigFile config;
        private string tempDir;

        [TestInitialize]
        public void Initialize()
        {
            StringBuilder file = new StringBuilder();
            file.AppendLine("[WindowSize]");
            file.AppendLine("Width = 1980");
            file.AppendLine("[Font]");
            file.AppendLine("Family = Comic Sans");

            tempDir = Path.GetTempPath();
            using (var writer = File.CreateText(Path.Combine(tempDir, "Editor.ini")))
            {
                writer.Write(file.ToString());
            }

            config = new ConfigFile("Editor.ini", new IniConfigProvider())
                        .AddLocation(tempDir, true);
        }


        [TestCleanup]
        public void Cleanup()
        {
            string overlay = Path.Combine(tempDir, "Editor.ini");
            if (File.Exists(overlay))
                File.Delete(overlay);
        }


        [TestMethod]
        public void GetUnmodified()
        {
            Assert.AreEqual("480", config.Get("WindowSize.Height"));
        }


        [TestMethod]
        public void GetModified()
        {
            Assert.AreEqual("Comic Sans", config.Get("Font.Family"));
            Assert.AreEqual("1980", config.Get("WindowSize.Width"));
        }


        [TestMethod]
        public void Save()
        {
            Assert.AreEqual("12", config.Get("AlternateFont.Size"));
            config.Set("AlternateFont.Size", "14");

            Assert.AreEqual(".\\Assets", config.Get("AssetsDir"));
            config.Set("AssetsDir", "Data");

            config.Save();

            config = new ConfigFile("Editor.ini", new IniConfigProvider())
                        .AddLocation(tempDir);

            Assert.AreEqual("14", config.Get("AlternateFont.Size"));
            Assert.AreEqual("Data", config.Get("AssetsDir"));
        }
    }
}
