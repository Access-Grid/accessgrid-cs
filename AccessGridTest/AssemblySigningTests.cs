using System.Reflection;
using NUnit.Framework;

namespace AccessGridTest;

[TestFixture]
public class AssemblySigningTests
{
#if SIGNING_ENABLED
    [Test]
    public void AccessGridAssembly_SignedBuild_ShouldHavePublicKey()
    {
        var assembly = typeof(AccessGrid.AccessGridClient).Assembly;
        var publicKey = assembly.GetName().GetPublicKey();

        Assert.That(publicKey, Is.Not.Null);
        Assert.That(publicKey, Is.Not.Empty);
    }
#else
    [Test]
    public void AccessGridAssembly_UnsignedBuild_ShouldNotHavePublicKey()
    {
        var assembly = typeof(AccessGrid.AccessGridClient).Assembly;
        var publicKey = assembly.GetName().GetPublicKey();

        Assert.That(publicKey, Is.Null.Or.Empty);
    }
#endif
}
