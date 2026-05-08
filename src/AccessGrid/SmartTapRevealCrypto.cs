using System;
using System.IO;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace AccessGrid
{
    internal static class SmartTapRevealCrypto
    {
        private const string CurveName = "P-256";
        private const string HkdfInfo = "accessgrid-smart-tap-reveal-v1";
        private const int AesKeyLengthBytes = 32;
        private const int SharedSecretLengthBytes = 32;

        internal sealed class GeneratedKeyPair
        {
            public AsymmetricCipherKeyPair KeyPair { get; }
            public string PublicKeyPem { get; }

            public GeneratedKeyPair(AsymmetricCipherKeyPair keyPair, string publicKeyPem)
            {
                KeyPair = keyPair;
                PublicKeyPem = publicKeyPem;
            }
        }

        public static GeneratedKeyPair GenerateP256KeyPair()
        {
            var curveParams = ECNamedCurveTable.GetByName(CurveName);
            var domain = new ECDomainParameters(curveParams.Curve, curveParams.G, curveParams.N, curveParams.H, curveParams.GetSeed());
            var keyGen = new ECKeyPairGenerator();
            keyGen.Init(new ECKeyGenerationParameters(domain, new SecureRandom()));
            var keyPair = keyGen.GenerateKeyPair();

            string publicKeyPem;
            using (var writer = new StringWriter())
            {
                var pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(keyPair.Public);
                pemWriter.Writer.Flush();
                publicKeyPem = writer.ToString();
            }

            return new GeneratedKeyPair(keyPair, publicKeyPem);
        }

        public static byte[] DecryptEnvelope(
            AsymmetricCipherKeyPair localKeyPair,
            string serverEphemeralPublicKeyPem,
            byte[] iv,
            byte[] ciphertext,
            byte[] tag)
        {
            var serverPublicKey = ReadEcPublicKeyFromPem(serverEphemeralPublicKeyPem);

            var sharedSecret = ComputeSharedSecret(localKeyPair.Private, serverPublicKey);
            var aesKey = DeriveAesKey(sharedSecret);
            return DecryptAesGcm(aesKey, iv, ciphertext, tag);
        }

        private static ECPublicKeyParameters ReadEcPublicKeyFromPem(string pem)
        {
            using var reader = new StringReader(pem);
            var pemReader = new PemReader(reader);
            var obj = pemReader.ReadObject();
            return obj switch
            {
                ECPublicKeyParameters ecPub => ecPub,
                AsymmetricCipherKeyPair pair when pair.Public is ECPublicKeyParameters pairPub => pairPub,
                _ => throw new InvalidOperationException("ephemeral_public_key is not a valid EC public key PEM")
            };
        }

        private static byte[] ComputeSharedSecret(AsymmetricKeyParameter privateKey, ECPublicKeyParameters publicKey)
        {
            var agreement = new ECDHBasicAgreement();
            agreement.Init(privateKey);
            var sharedBigInt = agreement.CalculateAgreement(publicKey);
            // Standard ECDH "raw" shared secret = X coordinate as fixed-width big-endian bytes (32 for P-256).
            return BigIntegerToFixedBytes(sharedBigInt, SharedSecretLengthBytes);
        }

        private static byte[] BigIntegerToFixedBytes(Org.BouncyCastle.Math.BigInteger value, int length)
        {
            var unsigned = value.ToByteArrayUnsigned();
            if (unsigned.Length == length) return unsigned;
            if (unsigned.Length > length)
                throw new InvalidOperationException("ECDH shared secret exceeds expected length");
            var padded = new byte[length];
            Buffer.BlockCopy(unsigned, 0, padded, length - unsigned.Length, unsigned.Length);
            return padded;
        }

        private static byte[] DeriveAesKey(byte[] sharedSecret)
        {
            var hkdf = new Org.BouncyCastle.Crypto.Generators.HkdfBytesGenerator(new Sha256Digest());
            var info = System.Text.Encoding.UTF8.GetBytes(HkdfInfo);
            hkdf.Init(new HkdfParameters(sharedSecret, salt: new byte[0], info: info));
            var output = new byte[AesKeyLengthBytes];
            hkdf.GenerateBytes(output, 0, output.Length);
            return output;
        }

        private static byte[] DecryptAesGcm(byte[] key, byte[] iv, byte[] ciphertext, byte[] tag)
        {
            var cipher = new GcmBlockCipher(new AesEngine());
            // BouncyCastle expects ciphertext||tag concatenated.
            var combined = new byte[ciphertext.Length + tag.Length];
            Buffer.BlockCopy(ciphertext, 0, combined, 0, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, combined, ciphertext.Length, tag.Length);

            cipher.Init(false, new AeadParameters(new KeyParameter(key), tag.Length * 8, iv, associatedText: new byte[0]));
            var output = new byte[cipher.GetOutputSize(combined.Length)];
            int len = cipher.ProcessBytes(combined, 0, combined.Length, output, 0);
            len += cipher.DoFinal(output, len);

            if (len == output.Length) return output;
            var trimmed = new byte[len];
            Buffer.BlockCopy(output, 0, trimmed, 0, len);
            return trimmed;
        }
    }
}
