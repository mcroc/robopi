using DevOpenSpace.RoboPi.Core.Enums;
using DevOpenSpace.RoboPi.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DevOpenSpace.RoboPi.Tests;

[TestClass]
public class IntentServiceTests
{
    private IntentService _intentService;

    [TestInitialize]
    public void Setup()
    {
        _intentService = new IntentService();
    }

    [TestMethod]
    public void GetMessage_StartEins()
    {
        var messages = _intentService.GetMessages("StartEins");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Start, messages[0].Intent);
        Assert.AreEqual(1, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_StoppZwei()
    {
        var messages = _intentService.GetMessages("StoppZwei");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Stopp, messages[0].Intent);
        Assert.AreEqual(2, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_RechtsDrei()
    {
        var messages = _intentService.GetMessages("RechtsDrei");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Rechts, messages[0].Intent);
        Assert.AreEqual(3, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_LinksZwei()
    {
        var messages = _intentService.GetMessages("LinksZwei");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Links, messages[0].Intent);
        Assert.AreEqual(2, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_VorwaertsDrei()
    {
        var messages = _intentService.GetMessages("VorwaertsDrei");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Vorwaerts, messages[0].Intent);
        Assert.AreEqual(3, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_RueckwaertsEins()
    {
        var messages = _intentService.GetMessages("RueckwaertsEins");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(Intent.Rueckwaerts, messages[0].Intent);
        Assert.AreEqual(1, messages[0].RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_WrongIntent()
    {
        var messages = _intentService.GetMessages("ObenDreizehn");
        Assert.IsNotNull(messages);
        Assert.IsFalse(messages.Any());
    }

    [TestMethod]
    public void GetMessage_StartAlle()
    {
        var messages = _intentService.GetMessages("StartAlle");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(3, messages.Count);
        Assert.IsTrue(messages.All(m => m.Intent == Intent.Start));
    }

    [TestMethod]
    public void GetMessage_StoppAlle()
    {
        var messages = _intentService.GetMessages("StoppAlle");
        Assert.IsNotNull(messages);
        Assert.IsTrue(messages.Any());
        Assert.AreEqual(3, messages.Count);
        Assert.IsTrue(messages.All(m => m.Intent == Intent.Stopp));
    }
}