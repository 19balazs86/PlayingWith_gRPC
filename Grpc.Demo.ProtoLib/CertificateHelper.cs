using System.Security.Cryptography.X509Certificates;

namespace Grpc.Demo.ProtoLib;

public static class CertificateHelper
{
    public static X509Certificate2 GetCertificate(string thumbprint)
    {
        // Get the current user personal certificate
        using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly);

        X509Certificate2 certificate = store.Certificates.FirstOrDefault(cert => validateByThumbprint(cert, thumbprint))
            ?? throw new Exception("Certificate is not found in the Current User store.");

        return certificate;
    }

    private static bool validateByThumbprint(X509Certificate2 certificate, string thumbprint)
    {
        return certificate.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase);
    }
}
