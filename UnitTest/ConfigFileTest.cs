using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ActLikeAI.Config.UnitTest
{
    [TestClass]
    public class ConfigFileTest
    {
        private ConfigFile config;

                
        [TestInitialize]
        public void Initialize()
        {
            config = new ConfigFile(@"Editor.cfg", new XmlConfigProvider());
        }


        [TestMethod]        
        public void GetSimpleOption()
        {
            Assert.AreEqual(@".\Assets", config.Get("AssetsDir"));
        }


        [TestMethod]
        public void GetNestedOption()
        {
            Assert.AreEqual("640", config.Get("MainWindow.Size.Width"));
        }


        [TestMethod]
        public void GetAttribute()
        {
            Assert.AreEqual("215", config.Get("MainWindow.Colors.Foreground.G"));            
        }


        [TestMethod]
        public void GetAsInt()
        {
            Assert.AreEqual(255, config.Get<int>("MainWindow.Colors.Foreground.R"));
        }


        [TestMethod]
        public void GetAsDouble()
        {
            Assert.AreEqual(18.5, config.Get<double>("MainWindow.Font.Size"), 1E-6);
        }


        [TestMethod]
        public void Set()
        {
            Assert.AreEqual("640", config.Get("MainWindow.Size.Width"));
            config.Set("MainWindow.Size.Width", "768");
            Assert.AreEqual("768", config.Get("MainWindow.Size.Width"));
        }


        [TestMethod]
        public void SetAsInt()
        {
            Assert.AreEqual("480", config.Get("MainWindow.Size.Height"));
            config.Set("MainWindow.Size.Height", 768);
            Assert.AreEqual("768", config.Get("MainWindow.Size.Height"));
        }
    }
}
