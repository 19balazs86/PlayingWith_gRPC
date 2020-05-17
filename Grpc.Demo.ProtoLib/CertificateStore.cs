using System;
using System.Security.Cryptography.X509Certificates;

namespace Grpc.Demo.ProtoLib
{
  public static class CertificateStore
  {
    public static X509Certificate2 GetCertificate(string thumbprint)
    {
      // Get the certificate store for the current user.
      using (X509Store store = new X509Store(StoreLocation.CurrentUser))
      {
        store.Open(OpenFlags.ReadOnly);

        X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

        if (certCollection.Count == 0)
          throw new Exception("Certificate is not found in the Current User store.");

        return certCollection[0];
      }
    }
  }
}
