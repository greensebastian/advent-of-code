using System.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace AdventOfCode2023.Core.Test;

static class ChromeManager
{
    private static string ChromeUserDataPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data");
    private static string ChromeCookiePath { get; } = Path.Combine(ChromeUserDataPath, "Default", "Network", "Cookies");
    private static string ChromeLocalStatePath { get; } = Path.Combine(ChromeUserDataPath, "Local State");
    
    public static List<Cookie> GetCookies(string hostname)
    {
        var data = new List<Cookie>();
        if (File.Exists(ChromeCookiePath))
        {
            try
            {
                using var conn = new SqliteConnection($"Data Source={ChromeCookiePath}");
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT name,encrypted_value,host_key FROM cookies WHERE host_key LIKE '%{hostname}%'";
                var key = AesGcm256.GetKey();

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (data.Any(a => a.Name == reader.GetString(0))) continue;

                        var encryptedData = GetBytes(reader, 1);
                        AesGcm256.Prepare(encryptedData, out var nonce, out var ciphertextTag);
                        var value = AesGcm256.Decrypt(ciphertextTag, key, nonce);

                        data.Add(new Cookie(reader.GetString(0), value));
                    }
                }

                conn.Close();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }
        return data;

    }

    private static byte[] GetBytes(IDataRecord reader, int columnIndex)
    {
        const int chunkSize = 2 * 1024;
        var buffer = new byte[chunkSize];
        long bytesRead;
        long fieldOffset = 0;
        using var stream = new MemoryStream();
        while ((bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, buffer.Length)) > 0)
        {
            stream.Write(buffer, 0, (int)bytesRead);
            fieldOffset += bytesRead;
        }
        return stream.ToArray();
    }

    public record Cookie(string Name, string Value);

    private static class AesGcm256
    {
        public static byte[] GetKey()
        {
            var v = File.ReadAllText(ChromeLocalStatePath);

            dynamic json = JsonConvert.DeserializeObject(v);
            string key = json.os_crypt.encrypted_key;

            var src = Convert.FromBase64String(key);
            var encryptedKey = src.Skip(5).ToArray();

            var decryptedKey = ProtectedData.Unprotect(encryptedKey, null, DataProtectionScope.CurrentUser);

            return decryptedKey;
        }

        public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            var sR = String.Empty;
            try
            {
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);
                var plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                var retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                sR = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return sR;
        }

        public static void Prepare(byte[] encryptedData, out byte[] nonce, out byte[] ciphertextTag)
        {
            nonce = new byte[12];
            ciphertextTag = new byte[encryptedData.Length - 3 - nonce.Length];

            Array.Copy(encryptedData, 3, nonce, 0, nonce.Length);
            Array.Copy(encryptedData, 3 + nonce.Length, ciphertextTag, 0, ciphertextTag.Length);
        }
    }
}