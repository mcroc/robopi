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
        var message = _intentService.GetMessage("StartEins");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Start, message.Intent);
        Assert.AreEqual(1, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_StoppZwei()
    {
        var message = _intentService.GetMessage("StoppZwei");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Stopp, message.Intent);
        Assert.AreEqual(2, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_RechtsDrei()
    {
        var message = _intentService.GetMessage("RechtsDrei");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Rechts, message.Intent);
        Assert.AreEqual(3, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_LinksZehn()
    {
        var message = _intentService.GetMessage("LinksZehn");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Links, message.Intent);
        Assert.AreEqual(10, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_VorwaertsSieben()
    {
        var message = _intentService.GetMessage("VorwaertsSieben");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Vorwaerts, message.Intent);
        Assert.AreEqual(7, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_RueckwaertsZwoelf()
    {
        var message = _intentService.GetMessage("RueckwaertsZwoelf");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Rueckwaerts, message.Intent);
        Assert.AreEqual(12, message.RoboPiNumber);
    }

    [TestMethod]
    public void GetMessage_WrongIntent()
    {
        var message = _intentService.GetMessage("ObenDreizehn");
        Assert.IsNotNull(message);
        Assert.AreEqual(Intent.Unbekannt, message.Intent);
        Assert.AreEqual(0, message.RoboPiNumber);
    }
}