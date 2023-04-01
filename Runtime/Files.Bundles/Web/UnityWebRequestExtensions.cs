﻿using UnityEngine.Networking;

namespace Depra.Assets.Runtime.Files.Bundles.Web
{
    internal static class UnityWebRequestExtensions
    {
        public static bool CanGetResult(this UnityWebRequest request) =>
            request.result != UnityWebRequest.Result.ProtocolError &&
            request.result != UnityWebRequest.Result.ConnectionError;
    }
}