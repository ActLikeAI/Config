using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigFileOverlayTest
    {
        private ConfigFile config;
        private string tempDir;

        [TestInitialize]
        public void Initialize()
        {
            XElement root = new XElement("Editor",
                                new XElement("MainWindow",
                                    new XElement("Font",
                                        new XElement("Family", "Consolas"),
                                        new XElement("Size", "16")),
                                    new XElement("Colors",
                                        new XElement("Background",
                                            new XAttribute("R", "128"),
                                            new XAttribute("G", "176"),
                                            new XAttribute("B", "208")))
                                    ));

            tempDir = Path.GetTempPath();
            using (var writer = File.CreateText(Path.Combine(tempDir, "Editor.cfg")))
            {
                root.Save(writer);
            }
            

            config = ConfigFile.Load("Editor.cfg", new XmlConfigProvider())
                        .AddLocation(tempDir, true);
        }


        [TestCleanup]
        public void Cleanup()
        {
            string overlay = Path.Combine(tempDir, "Editor.cfg");
            if (File.Exists(overlay))
                File.Delete(overlay);
        }


        [TestMethod]
        public void GetUnmodified()
        {
            Assert.AreEqual("0", config.Get("MainWindow.Colors.Foreground.B"));
        }


        [TestMethod]
        public void GetModified()
        {
            Assert.AreEqual("Consolas", config.Get("MainWindow.Font.Family"));
            Assert.AreEqual("176", config.Get("MainWindow.Colors.Background.G"));
        }


        [TestMethod]
        public void Save()
        {
            Assert.AreEqual("0", config.Get("MainWindow.Colors.Foreground.B"));

            config.Set("MainWindow.Colors.Foreground.B", "11");
            config.Save();

            config = ConfigFile.Load("Editor.cfg", new XmlConfigProvider())
                        .AddLocation(tempDir);

            Assert.AreEqual("11", config.Get("MainWindow.Colors.Foreground.B"));
        }
    }
}
