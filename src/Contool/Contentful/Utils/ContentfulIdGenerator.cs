﻿using System.Security.Cryptography;
using System.Text;

namespace Contool.Contentful.Utils;

internal class ContentfulIdGenerator
{
    private const int IdLength = 22;
    private const string CharSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public static string NewId()
    {
        var result = new StringBuilder("contool-", IdLength + 8); // prefix + ID
        var buffer = new byte[IdLength];

        Rng.GetBytes(buffer);

        foreach (var b in buffer)
        {
            result.Append(CharSet[b % CharSet.Length]);
        }

        return result.ToString();
    }
}
