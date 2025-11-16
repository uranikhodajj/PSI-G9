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
            var hash = sha.ComputeHash(fs);
            return ToHex(hash);
        }

        private static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static bool RunSelfTests()
        {
            Console.WriteLine("=== NIST test vectors për SHA-256 ===");
            bool nistOk = RunNistTestVectors();
            Console.WriteLine();

            TestDeterminism();
            Console.WriteLine();

            TestAvalancheEffect();
            Console.WriteLine();

            Benchmarking();
            Console.WriteLine();

            TestCollisions();
            Console.WriteLine();

            Console.WriteLine("=== Self-tests të përdoruesit për SHA-256 ===");
            var vectors = new Dictionary<string, string>
            {
                [""] = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                ["abc"] = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad",
                ["The quick brown fox jumps over the lazy dog"] = "d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592",
                ["The quick brown fox jumps over the lazy dog."] = "ef537f25c895bfa782526529a9b63d97aa631564d5d789c2b765448c8635fb6c",
                ["Hash me, Shqipëri: ëË"] = "bcaa3ab6009c7c62a26b06ab7a18ad795c6e32f4421a39eabc5e85596a5b5593"
            };

            int passed = 0, failed = 0;

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
            return failed == 0 && nistOk;
        }

        private static string DisplaySample(string s)
        {
            if (s.Length <= 40) return s;
            return s.Substring(0, 37) + "...";
        }

        private static bool RunNistTestVectors()
        {
            var nistVectors = new Dictionary<string, string>
            {
                ["abc"] = "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad",
                [""] = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
                ["abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"] =
                    "248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1"
            };

            int passed = 0, failed = 0;

            foreach (var kv in nistVectors)
            {
                string input = kv.Key;
                string expected = kv.Value;

                string actual = Sha256Hex(Encoding.UTF8.GetBytes(input));
                bool ok = string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);

                if (ok)
                {
                    passed++;
                }
                else
                {
                    failed++;
                    Console.WriteLine($"NIST FAIL | \"{DisplaySample(input)}\"");
                    Console.WriteLine($"          Expected: {expected}");
                    Console.WriteLine($"          Actual  : {actual}");
                }
            }

            Console.WriteLine($"NIST Rezultati: {passed} kaluan, {failed} dështuan.");
            return failed == 0;
        }

        private static void TestDeterminism()
        {
            Console.WriteLine("=== Testimi i determinizmit ===");
            var inputs = new[]
            {
                "",
                "abc",
                "The quick brown fox jumps over the lazy dog"
            };

            foreach (var input in inputs)
            {
                string hash1 = Sha256Hex(Encoding.UTF8.GetBytes(input));
                string hash2 = Sha256Hex(Encoding.UTF8.GetBytes(input));
                bool ok = hash1 == hash2;

                Console.WriteLine($"{DisplaySample(input)} -> Hash1: {hash1}, Hash2: {hash2} | {(ok ? "OK" : "FAIL")}");
            }
        }
        
        private static void TestAvalancheEffect()
        {
            Console.WriteLine("=== Testimi i efektit Avalanche ===");
            var input = "The quick brown fox jumps over the lazy dog";
            string baseHash = Sha256Hex(Encoding.UTF8.GetBytes(input));

            char[] chars = input.ToCharArray();
            chars[0] = chars[0] == 'T' ? 't' : 'T';
            string modified = new string(chars);

            string modifiedHash = Sha256Hex(Encoding.UTF8.GetBytes(modified));

            Console.WriteLine($"Origjinali : {baseHash}");
            Console.WriteLine($"Modifikuari: {modifiedHash}");

            int diffBits = CountDifferentBits(baseHash, modifiedHash);
            Console.WriteLine($"Numri i bitëve të ndryshëm: {diffBits}");
        }

        private static int CountDifferentBits(string hex1, string hex2)
        {
            byte[] bytes1 = Convert.FromHexString(hex1);
            byte[] bytes2 = Convert.FromHexString(hex2);

            int count = 0;
            for (int i = 0; i < bytes1.Length; i++)
            {
                byte xor = (byte)(bytes1[i] ^ bytes2[i]);
                count += CountBits(xor);
            }
            return count;
        }

        private static int CountBits(byte b)
        {
            int count = 0;
            while (b != 0)
            {
                count += b & 1;
                b >>= 1;
            }
            return count;
        }

        private static void Benchmarking()
        {
            Console.WriteLine("=== Benchmarking SHA-256 ===");

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            Sha256Hex(Encoding.UTF8.GetBytes("benchmark test"));
            sw.Stop();
            Console.WriteLine($"1 hash: {sw.Elapsed.TotalMilliseconds:F3} ms");

            sw.Restart();
            for (int i = 0; i < 1000; i++)
                Sha256Hex(Encoding.UTF8.GetBytes("input " + i));
            sw.Stop();
            Console.WriteLine($"1000 hash-e: {sw.Elapsed.TotalMilliseconds:F3} ms");

            double seconds = sw.Elapsed.TotalSeconds;
            if (seconds > 0)
            {
                double hashesPerSec = 1000.0 / seconds;
                Console.WriteLine($"Shpejtësia: {hashesPerSec:F2} hash/sec");
            }
            else
            {
                Console.WriteLine("Shpejtësia: shumë e lartë për t'u matur me këtë rezolucion kohor.");
            }
        }

        private static void TestCollisions()
        {
            Console.WriteLine("=== Testimi i kolizioneve ===");

            var inputs = new[]
            {
                "",
                "a",
                "A",
                "abc",
                "Abc",
                "123",
                "!@#",
                "The quick brown fox",
                "The quick brown fox.",
                "Hash me, Shqipëri: ëË"
            };

            var hashes = new Dictionary<string, string>();
            bool collisionFound = false;

            foreach (var input in inputs)
            {
                string h = Sha256Hex(Encoding.UTF8.GetBytes(input));

                if (hashes.ContainsKey(h))
                {
                    collisionFound = true;
                    Console.WriteLine($"KOLIZION! \"{input}\" dhe \"{hashes[h]}\" → {h}");
                }
                else
                {
                    hashes[h] = input;
                }
            }

            if (!collisionFound)
                Console.WriteLine("Asnjë kolizion nuk u gjet.");
        }
    }
}
