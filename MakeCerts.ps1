Write-Host "Creating Certificates for Self-Signed Testing"

Write-Host "Creating Root Certificate"
$rootCA = New-SelfSignedCertificate -Type Custom -KeySpec Signature `
-Subject "CN=localhost" `
-FriendlyName "GrpcDemoRootCert" `
-KeyExportPolicy Exportable `
-HashAlgorithm sha256 -KeyLength 4096 `
-CertStoreLocation "Cert://CurrentUser/My" `
-KeyUsageProperty Sign `
-KeyUsage CertSign `
-NotAfter (Get-Date).AddYears(5)

# Client Auth
Write-Host "Creating Client Auth Certificate"
$clientCert = New-SelfSignedCertificate -Type Custom -KeySpec Signature `
-Subject "CN=localhost" -KeyExportPolicy Exportable `
-FriendlyName "GrpcDemoClientCert" `
-HashAlgorithm sha256 -KeyLength 2048 `
-NotAfter (Get-Date).AddMonths(24) `
-CertStoreLocation "Cert://CurrentUser/My" `
-Signer $rootCA -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.2")

# TLS Cert
Write-Host "Creating Web Server Certificate"
$serverCert = New-SelfSignedCertificate -Type Custom `
-Subject "CN=localhost" -KeyExportPolicy Exportable `
-DnsName "localhost" `
-FriendlyName "GrpcDemoServerCert" `
-HashAlgorithm sha256 -KeyLength 2048 `
-KeyUsage "KeyEncipherment", "DigitalSignature" `
-NotAfter (Get-Date).AddMonths(24) `
-CertStoreLocation "Cert://CurrentUser/My" `
-Signer $rootCA

# You need to move the root cert to the Trusted Root. Or you can use the certmgr tool.
Write-Host "Move Root Certificate to the Trusted Root"
$rootCAStore = New-Object -TypeName `
  System.Security.Cryptography.X509Certificates.X509Store(
  [System.Security.Cryptography.X509Certificates.StoreName]::Root,
  'CurrentUser')
$rootCAStore.open('MaxAllowed')
$rootCAStore.add($rootCA)
$rootCAStore.close()

# You can export client and server certs if you need. But You can read them from the store.
$PFXPass = ConvertTo-SecureString -String "P@ssw0rd!" -Force -AsPlainText

Write-Host "Exporting Certificates to File"

Export-PfxCertificate -Cert $clientCert `
-Password $PFXPass `
-FilePath GrpcDemoClientSelfCert.pfx

Export-PfxCertificate -Cert $serverCert `
-Password $PFXPass `
-FilePath GrpcDemoServerCert.pfx

