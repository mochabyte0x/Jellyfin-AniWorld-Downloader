using System;
using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.AniWorld.Helpers;

/// <summary>Strict public cover proxy allowlist, independent of configurable provider URLs.</summary>
public static class CoverUrlValidator
{
    private static readonly Regex MegaKinoHost = new(@"^megakino\d*\.com$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>Accept only HTTPS image URLs on known provider hosts.</summary>
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrEmpty(value) || value.Length > 2048 ||
            !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps || uri.Port != 443 ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            return false;
        }

        var host = uri.Host.ToLowerInvariant();
        var allowed = host is "aniworld.to" or "www.aniworld.to" or
            "s.to" or "www.s.to" or "serienstream.to" or "www.serienstream.to" or
            "filmo.to" or "www.filmo.to" or "filmpalast.to" or "www.filmpalast.to" or
            "moflix-stream.xyz" or "www.moflix-stream.xyz" || MegaKinoHost.IsMatch(host);
        return allowed && uri.AbsolutePath.StartsWith("/", StringComparison.Ordinal) &&
            (uri.AbsolutePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
             uri.AbsolutePath.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
             uri.AbsolutePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
             uri.AbsolutePath.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ||
             uri.AbsolutePath.EndsWith(".gif", StringComparison.OrdinalIgnoreCase));
    }
}
