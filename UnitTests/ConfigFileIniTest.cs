using ActLikeAI.Config.Ini;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;


namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigFileIniTest
    {
        private ConfigFile config;

        [TestInitialize]
        public void Initialize()
        {
            config = new ConfigFile(@"Editor.ini", new IniConfigProvider());
        }


        [TestMethod]
        public void GetSimpleOption()
        {
            Assert.AreEqual(@".\Assets", config.Get("AssetsDir"));
        }


        [TestMethod]
        public void GetNestedOption()
        {
            Assert.AreEqual("640", config.Get("WindowSize.Width"));
            Assert.AreEqual("480", config.Get("WindowSize.Height"));
        }


        [TestMethod]
        public void GetAsInt()
        {
            Assert.AreEqual(12, config.Get<int>("AlternateFont.Size"));
        }


        [TestMethod]
        public void GetAsDouble()
        {
            Assert.AreEqual(14.5, config.Get<double>("Font.Size"), 1E-6);
        }


        [TestMethod]
        public void Set()
        {
            Assert.AreEqual("Consolas", config.Get("AlternateFont.Family"));
            config.Set("AlternateFont.Family", "Comic Sans");
            Assert.AreEqual("Comic Sans", config.Get("AlternateFont.Family"));
        }


        [TestMethod]
        public void SetAsInt()
        {
            Assert.AreEqual("640", config.Get("WindowSize.Width"));
            config.Set("WindowSize.Width", "320");
            Assert.AreEqual("320", config.Get("WindowSize.Width"));
        }


    }
}
