using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigManagerTest
    {
        ConfigManager manager;

        [TestInitialize]
        public void Initialize()
        {
            var provider = new XmlConfigProvider();

            var editor = new ConfigFile("Editor.cfg", provider);
            var model = new ConfigFile("Model.cfg", provider);

            manager = new ConfigManager()
                .Add(editor)
                .Add(model);
        }

        [TestMethod]
        public void GetSimpleOption()
        {
            Assert.AreEqual(@".\Assets", manager.Get("Editor.AssetsDir"));
            Assert.AreEqual("Blue", manager.Get("Model.Background"));
        }


        [TestMethod]
        public void GetNestedOption()
        {
            Assert.AreEqual("640", manager.Get("Editor.MainWindow.Size.Width"));
            Assert.AreEqual("Icosahedron", manager.Get("Model.Mesh.StartingShape"));
        }


        [TestMethod]
        public void GetAttribute()
        {
            Assert.AreEqual("215", manager.Get("Editor.MainWindow.Colors.Foreground.G"));
            Assert.AreEqual("Frequency", manager.Get("Model.Mesh.Subdivision.Type"));
        }


        [TestMethod]
        public void GetAsInt()
        {
            Assert.AreEqual(255, manager.Get<int>("Editor.MainWindow.Colors.Foreground.R"));
            Assert.AreEqual(17, manager.Get<int>("Model.Mesh.Subdivision.Level"));
        }


        [TestMethod]
        public void Set()
        {
            Assert.AreEqual("640", manager.Get("Editor.MainWindow.Size.Width"));
            manager.Set("Editor.MainWindow.Size.Width", "768");
            Assert.AreEqual("768", manager.Get("Editor.MainWindow.Size.Width"));

            Assert.AreEqual("Frequency", manager.Get("Model.Mesh.Subdivision.Type"));
            manager.Set("Model.Mesh.Subdivision.Type", "Depth");
            Assert.AreEqual("Depth", manager.Get("Model.Mesh.Subdivision.Type"));
        }


        [TestMethod]
        public void SetAsInt()
        {
            Assert.AreEqual("480", manager.Get("Editor.MainWindow.Size.Height"));
            manager.Set("Editor.MainWindow.Size.Height", 768);
            Assert.AreEqual("768", manager.Get("Editor.MainWindow.Size.Height"));

            Assert.AreEqual(17, manager.Get<int>("Model.Mesh.Subdivision.Level"));
            manager.Set("Model.Mesh.Subdivision.Level", 5);
            Assert.AreEqual(5, manager.Get<int>("Model.Mesh.Subdivision.Level"));
        }
    }
}
