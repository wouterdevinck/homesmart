using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Adapted from https://github.com/ClusterM/tuyanet

namespace Home.Devices.Tuya.Api {

    internal static class TuyaParser {
        
        private static readonly byte[] ProtocolVersionBytes33 = "3.3"u8.ToArray();
        private static readonly byte[] Protocol33Header = ProtocolVersionBytes33.Concat(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }).ToArray();
        private static readonly byte[] Prefix = { 0, 0, 0x55, 0xAA };
        private static readonly byte[] Suffix = { 0, 0, 0xAA, 0x55 };

        private static uint _seqNo;

        internal static IEnumerable<byte> BigEndian(IEnumerable<byte> seq) => BitConverter.IsLittleEndian ? seq.Reverse() : seq;

        internal static byte[] Encrypt(byte[] data, byte[] key) {
            using var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = key;
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();
            return ms.ToArray();
        }

        internal static byte[] Decrypt(byte[] data, byte[] key) {
            if (data.Length == 0) return data;
            using var aes = Aes.Create();
            aes.Mode = CipherMode.ECB;
            aes.Key = key;
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.Close();
            return ms.ToArray();
        }

        internal static byte[] EncodeRequest(TuyaCommand command, string json, byte[] key) {
            var root = JObject.Parse(json);
            json = root.ToString(Formatting.None);
            var payload = Encoding.UTF8.GetBytes(json);
            payload = Encrypt(payload, key);
            if (command != TuyaCommand.DpQuery) {
                payload = Protocol33Header.Concat(payload).ToArray();
            }
            using var ms = new MemoryStream();
            var seqNo = BitConverter.GetBytes(_seqNo++);
            if (BitConverter.IsLittleEndian) Array.Reverse(seqNo);
            var dataLength = BitConverter.GetBytes(payload.Length + 8);
            if (BitConverter.IsLittleEndian) Array.Reverse(dataLength);
            ms.Write(Prefix, 0, 4);
            ms.Write(seqNo, 0, 4);
            ms.Write(new byte[] { 0, 0, 0, (byte)command }, 0, 4);
            ms.Write(dataLength, 0, 4);
            ms.Write(payload, 0, payload.Length);
            var crc32 = new TuyaCrc32();
            var crc = crc32.Get(ms.ToArray());
            var crcBin = BitConverter.GetBytes(crc);
            if (BitConverter.IsLittleEndian) Array.Reverse(crcBin);
            ms.Write(crcBin, 0, 4);
            ms.Write(Suffix, 0, 4);
            return ms.ToArray();
        }
        
        internal static TuyaResponse DecodeResponse(byte[] data, byte[] key) {
            if (data.Length < 20 || !data.Take(Prefix.Length).SequenceEqual(Prefix)) {
                throw new InvalidDataException("Invalid header/prefix");
            }
            var length = BitConverter.ToInt32(BigEndian(data.Skip(12).Take(4)).ToArray(), 0);
            if (!data.Skip(16 + length - Suffix.Length).Take(Suffix.Length).SequenceEqual(Suffix)) {
                throw new InvalidDataException("Invalid suffix");
            }
            var command = (TuyaCommand)BitConverter.ToUInt32(BigEndian(data.Skip(8).Take(4)).ToArray(), 0);
            var returnCode = BitConverter.ToInt32(BigEndian(data.Skip(16).Take(4)).ToArray(), 0);
            data = data.Skip(20).Take(length - 12).ToArray();
            if (data.Take(ProtocolVersionBytes33.Length).SequenceEqual(ProtocolVersionBytes33)) {
                data = data.Skip(Protocol33Header.Length).ToArray();
            }
            data = Decrypt(data, key);
            if (data.Length == 0) {
                return new TuyaResponse(command, returnCode, string.Empty);
            }
            var json = Encoding.UTF8.GetString(data);
            return new TuyaResponse(command, returnCode, json);
        }

    }

}