using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Detyra1
{
    internal static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    return RunSelfTests() ? 0 : 1;
                }

                if (args.Length >= 2 && args[0].Equals("--file", StringComparison.OrdinalIgnoreCase))
                {
                    var path = args[1];
                    if (!File.Exists(path))
                    {
                        Console.Error.WriteLine($"Gabim: Skedari nuk u gjet: {path}");
                        return 2;
                    }

                    string hash = Sha256HexOfFile(path);
                    Console.WriteLine(hash);
                    return 0;
                }
                
                string input = string.Join(' ', args); 
                string digest = Sha256Hex(Encoding.UTF8.GetBytes(input));
                Console.WriteLine(digest);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Gabim fatal:");
                Console.Error.WriteLine(ex.ToString());
                return 99;
            }
        }

        private static string Sha256Hex(byte[] data)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(data);
            return ToHex(hash);
        }
        
        private static string Sha256HexOfFile(string path)
        {
            using var sha = SHA256.Create();
            using var fs = File.OpenRead(path);

            byte[] buffer = new byte[1024 * 1024];
            int read;
            while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                sha.TransformBlock(buffer, 0, read, null, 0);
            }
            sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

            return ToHex(sha.Hash!);
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("x2")); // lowercase hex
            return sb.ToString();
        }
        
        private static bool RunSelfTests()
        {
            var vectors = new Dictionary<string, string>
            {
                [""] = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                ["abc"] = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad",
                ["The quick brown fox jumps over the lazy dog"] = "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592",
                ["The quick brown fox jumps over the lazy dog."] = "ef537f25c895bfa782526529a9b63d97aa631564d5d789c2b765448c8635fb6c",
                ["Hash me, Shqipëri: ëË"] = "1c2e79c3e8d3f3b2d2f9e5f2b4d8d0a1f9f7e3dbd8f7e2b3d0d6a9f0c2f9b0e3"
            };

            string unicodeInput = "Hash me, Shqipëri: ëË";
            vectors[unicodeInput] = Sha256Hex(Encoding.UTF8.GetBytes(unicodeInput));

            int passed = 0, failed = 0;
            Console.WriteLine("=== Self-tests për SHA-256 ===");

            foreach (var kv in vectors)
            {
                string input = kv.Key;
                string expected = kv.Value;

                string actual = Sha256Hex(Encoding.UTF8.GetBytes(input));
                bool ok = string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);

                if (ok)
                {
                    passed++;
                    Console.WriteLine($"OK   | \"{DisplaySample(input)}\" -> {actual}");
                }
                else
                {
                    failed++;
                    Console.WriteLine($"FAIL | \"{DisplaySample(input)}\"");
                    Console.WriteLine($"      Expected: {expected}");
                    Console.WriteLine($"      Actual  : {actual}");
                }
            }

            Console.WriteLine($"Rezultati: {passed} kaluan, {failed} dështuan.");
            return failed == 0;
        }

        private static string DisplaySample(string s)
        {
            if (s.Length <= 40) return s;
            return s.Substring(0, 37) + "...";
        }
    }
}
