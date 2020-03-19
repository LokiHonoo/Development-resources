﻿using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.IO;
using MicrosoftX509 = System.Security.Cryptography.X509Certificates;

namespace LH.BouncyCastleHelpers
{
    /// <summary>
    /// 测试。
    /// </summary>
    internal static class Test
    {
        /// <summary>
        /// 证书相关测试。
        /// </summary>
        internal static void TestCert()
        {
            //
            // ===========================  密钥测试  ===========================
            //
            var caKeyPair = CryptoHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);
            var serverKeyPair = CryptoHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);
            var clientKeyPair = CryptoHelper.GenerateEcdsaKeyPair(NamedCurves.SECP256R1);
            //
            // 密钥读写测试。
            //
            CryptoHelper.ParseKey(caKeyPair.Private, out string privateKeyPem);
            CryptoHelper.ParseKey(caKeyPair.Private, NamedPemEncryptionAlgorithms.AES_256_CBC, "123456", out string privateKeyEncPem);
            CryptoHelper.ParseKey(caKeyPair.Public, out string publicKeyPem);
            //
            caKeyPair = CryptoHelper.ReadKeyPair(privateKeyPem);
            caKeyPair = CryptoHelper.ReadKeyPair(privateKeyEncPem, "123456");
            _ = CryptoHelper.ReadPublicKey(publicKeyPem);
            //
            // ===========================  证书测试  ===========================
            //
            var caDN = CertificateHelper.GenerateDN(
                new Dictionary<DerObjectIdentifier, string>(2)
                {
                    { X509Name.C, "CN" },
                    { X509Name.CN, "LH.Net.Sockets TEST Root CA" }
                });
            var serverDN = CertificateHelper.GenerateDN(
                new Dictionary<DerObjectIdentifier, string>(2)
                {
                    { X509Name.C, "CN" },
                    { X509Name.CN, "LH.Net.Sockets TEST TCP Server" }
                });
            var clientDN = CertificateHelper.GenerateDN(
                new Dictionary<DerObjectIdentifier, string>(2)
                {
                    { X509Name.C, "CN" },
                    { X509Name.CN, "LH.Net.Sockets TEST TCP Client" }
                });
            //
            // 机构证书。
            //
            var caCert = CertificateHelper.GenerateIssuerCert(NamedSignatureAlgorithms.SHA256_WITH_ECDSA,
                                                              caKeyPair,
                                                              caDN,
                                                              null,
                                                              DateTime.UtcNow.AddDays(-1),
                                                              365);
            //
            // P12 证书。
            //
            //
            using (MemoryStream ms = new MemoryStream())
            {
                CertificateHelper.GeneratePfx(caKeyPair.Private, new X509Certificate[] { caCert }, "123456", ms);
            }
            //
            // 使用者证书请求。
            //
            var serverCsr = CertificateHelper.GenerateCsr(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, serverKeyPair, serverDN, null);
            var clientCsr = CertificateHelper.GenerateCsr(NamedSignatureAlgorithms.SHA256_WITH_ECDSA, clientKeyPair, clientDN, null);
            //
            // 证书请求读写测试。
            //
            CertificateHelper.ParseCsr(serverCsr, out string serverCsrPem);
            CertificateHelper.ParseCsr(clientCsr, out string clientCsrPem);
            serverCsr = CertificateHelper.ReadCsr(serverCsrPem);
            clientCsr = CertificateHelper.ReadCsr(clientCsrPem);
            //
            // 使用者证书。
            //
            var serverCert = CertificateHelper.GenerateSubjectCert(NamedSignatureAlgorithms.SHA256_WITH_ECDSA,
                                                                   caKeyPair.Private,
                                                                   caCert,
                                                                   serverCsr,
                                                                   null,
                                                                   DateTime.UtcNow.AddDays(-1),
                                                                   365);
            var clientCert = CertificateHelper.GenerateSubjectCert(NamedSignatureAlgorithms.SHA256_WITH_ECDSA,
                                                                   caKeyPair.Private,
                                                                   caCert,
                                                                   clientCsr,
                                                                   null,
                                                                   DateTime.UtcNow.AddDays(-1),
                                                                   365);
            //
            // 证书读写测试。
            //
            CertificateHelper.ParseCert(caCert, out string caCertPem);
            CertificateHelper.ParseCert(serverCert, out string serverCertPem);
            CertificateHelper.ParseCert(clientCert, out string clientCertPem);
            caCert = CertificateHelper.ReadCert(caCertPem);
            serverCert = CertificateHelper.ReadCert(serverCertPem);
            clientCert = CertificateHelper.ReadCert(clientCertPem);
            //
            // 打印证书。
            //
            Console.WriteLine("===========================  CA Cert  ===========================");
            Console.WriteLine(caCert.ToString());
            Console.WriteLine("=========================  Server Cert   ========================");
            Console.WriteLine(serverCert.ToString());
            Console.WriteLine("=========================  Client Cert   ========================");
            Console.WriteLine(clientCert.ToString());
            Console.WriteLine();
            //
            // BouncyCastle 证书验证。
            //
            bool validated;
            try
            {
                serverCert.Verify(caCert.GetPublicKey());
                validated = true;
            }
            catch
            {
                validated = false;
            }
            Console.WriteLine("BouncyCastle verify server cert - " + validated);
            try
            {
                clientCert.Verify(caCert.GetPublicKey());
                validated = true;
            }
            catch
            {
                validated = false;
            }
            Console.WriteLine("BouncyCastle verify client cert - " + validated);
            //
            // Microsoft 证书链验证。
            //
            var caCertMS = new MicrosoftX509.X509Certificate2(caCert.GetEncoded());
            var serverCertMS = new MicrosoftX509.X509Certificate2(serverCert.GetEncoded());
            var clientCertMS = new MicrosoftX509.X509Certificate2(clientCert.GetEncoded());
            var chain = new MicrosoftX509.X509Chain();
            chain.ChainPolicy.RevocationMode = MicrosoftX509.X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = MicrosoftX509.X509VerificationFlags.AllowUnknownCertificateAuthority;
            chain.ChainPolicy.ExtraStore.Add(caCertMS);
            Console.WriteLine("Microsoft chain verify server cert - " + chain.Build(serverCertMS));
            Console.WriteLine("Microsoft chain verify client cert - " + chain.Build(clientCertMS));
            (caCertMS as IDisposable).Dispose();
            (serverCertMS as IDisposable).Dispose();
            (clientCertMS as IDisposable).Dispose();
            (chain as IDisposable).Dispose();
            //
            // =============================  签名  =============================
            //
            var data = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==========================  Signature   =========================");
            var signature = SignatureHelper.Sign(serverKeyPair.Private, NamedSignatureAlgorithms.SHA256_WITH_ECDSA, data);
            validated = SignatureHelper.Verify(serverCert.GetPublicKey(), NamedSignatureAlgorithms.SHA256_WITH_ECDSA, data, signature);
            Console.WriteLine("Server terminal verify signature - " + validated);
            signature = SignatureHelper.Sign(clientKeyPair.Private, NamedSignatureAlgorithms.SHA256_WITH_ECDSA, data);
            validated = SignatureHelper.Verify(clientCert.GetPublicKey(), NamedSignatureAlgorithms.SHA256_WITH_ECDSA, data, signature);
            Console.WriteLine("Client terminal verify signature - " + validated);
            //
            // ===========================  密钥交换  ===========================
            //
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("=========================  KeyExchange   ========================");
            var parametersA = KeyExchangeHelper.CreateParametersA(256);
            var agreementA = KeyExchangeHelper.CreateAgreement(parametersA, out AsymmetricKeyParameter publicKeyA);
            var parametersB = KeyExchangeHelper.CreateParametersB(parametersA.P, parametersA.G);
            var agreementB = KeyExchangeHelper.CreateAgreement(parametersB, out AsymmetricKeyParameter publicKeyB);
            var pmsA = agreementA.CalculateAgreement(publicKeyB);
            var pmsB = agreementB.CalculateAgreement(publicKeyA);
            Console.WriteLine("Key exchange alice pms - ");
            Console.WriteLine(pmsA);
            Console.WriteLine("Key exchange bob pms - ");
            Console.WriteLine(pmsB);
            //
            //
            //
            Console.ReadKey(true);
        }
    }
}