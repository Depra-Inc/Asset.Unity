﻿// SPDX-License-Identifier: Apache-2.0
// © 2023 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Extensions;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Sources
{
	public readonly struct AssetBundleFromStream : IAssetBundleSource
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Stream OpenStream(string path) =>
			new FileStream(path, FileMode.Open, FileAccess.Read);

		FileSize IAssetBundleSource.Size(AssetBundle of) => of.Size();

		AssetBundle IAssetBundleSource.Load(string by)
		{
			Guard.AgainstFileNotFound(by);

			using var fileStream = OpenStream(by);

			return AssetBundle.LoadFromStream(fileStream);
		}

		async Task<AssetBundle> IAssetBundleSource.LoadAsync(string by, Action<float> withProgress,
			CancellationToken cancellationToken)
		{
			Guard.AgainstFileNotFound(by);

			await using var stream = OpenStream(by);

			return await AssetBundle
				.LoadFromStreamAsync(stream)
				.ToTask(withProgress, cancellationToken);
		}
	}
}