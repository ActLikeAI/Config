using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigManagerSingleFileTest
    {
        ConfigManager manager;

        [TestInitialize]
        public void Initialize()
        {
            manager = new ConfigManager()
                .Add(new ConfigFile(@"Editor.cfg"));
        }


        [TestMethod]        
        public void GetSimpleOption()
        {
            Assert.AreEqual(@".\Assets", manager.Get("Editor.AssetsDir"));
            Assert.AreEqual(@".\Assets", manager.Get("AssetsDir"));            
        }


        [TestMethod]
        public void GetNestedOption()
        {
            Assert.AreEqual("640", manager.Get("Editor.MainWindow.Size.Width"));
            Assert.AreEqual("640", manager.Get("MainWindow.Size.Width"));
        }


        [TestMethod]
        public void GetAttribute()
        {
            Assert.AreEqual("215", manager.Get("Editor.MainWindow.Colors.Foreground.G"));
            Assert.AreEqual("215", manager.Get("MainWindow.Colors.Foreground.G"));
        }


        [TestMethod]
        public void GetAsInt()
        {
            Assert.AreEqual(255, manager.Get<int>("Editor.MainWindow.Colors.Foreground.R"));
            Assert.AreEqual(255, manager.Get<int>("MainWindow.Colors.Foreground.R"));
        }


        [TestMethod]
        public void GetAsDouble()
        {
            Assert.AreEqual(18.5, manager.Get<double>("Editor.MainWindow.Font.Size"), 1E-6);
            Assert.AreEqual(18.5, manager.Get<double>("MainWindow.Font.Size"), 1E-6);
        }


        [TestMethod]
        public void Set()
        {
            Assert.AreEqual("640", manager.Get("MainWindow.Size.Width"));
            manager.Set("Editor.MainWindow.Size.Width", "768");
            Assert.AreEqual("768", manager.Get("MainWindow.Size.Width"));            
            manager.Set("MainWindow.Size.Width", "1024");
            Assert.AreEqual("1024", manager.Get("Editor.MainWindow.Size.Width"));
        }


        [TestMethod]
        public void SetAsInt()
        {
            Assert.AreEqual("480", manager.Get("MainWindow.Size.Height"));
            manager.Set("Editor.MainWindow.Size.Height", 768);
            Assert.AreEqual("768", manager.Get("MainWindow.Size.Height"));
            manager.Set("Editor.MainWindow.Size.Height", 1024);
            Assert.AreEqual("1024", manager.Get("MainWindow.Size.Height"));
        }
    }   
}
