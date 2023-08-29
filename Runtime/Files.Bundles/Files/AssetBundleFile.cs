﻿// Copyright © 2023 Nikolay Melnikov. All rights reserved.
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Depra.Assets.Delegates;
using Depra.Assets.Idents;
using Depra.Assets.Runtime.Exceptions;
using Depra.Assets.Runtime.Files.Adapter;
using Depra.Assets.Runtime.Files.Bundles.Exceptions;
using Depra.Assets.Runtime.Files.Bundles.Idents;
using Depra.Assets.Runtime.Files.Bundles.Sources;
using Depra.Assets.ValueObjects;
using UnityEngine;

namespace Depra.Assets.Runtime.Files.Bundles.Files
{
	public sealed class AssetBundleFile : UnityAssetFile<AssetBundle>, IDisposable
	{
		public static implicit operator AssetBundle(AssetBundleFile from) => from.Load();

		private readonly AssetBundleIdent _ident;
		private readonly IAssetBundleSource _source;

		private AssetBundle _loadedAssetBundle;

		public AssetBundleFile(AssetBundleIdent ident, IAssetBundleSource source)
		{
			_ident = ident ?? throw new ArgumentNullException(nameof(ident));
			_source = source ?? throw new ArgumentNullException(nameof(source));
		}

		public override IAssetIdent Ident => _ident;
		public override bool IsLoaded => _loadedAssetBundle != null;
		public override FileSize Size { get; protected set; } = FileSize.Unknown;

		public override AssetBundle Load()
		{
			if (IsLoaded)
			{
				return _loadedAssetBundle;
			}

			var loadedAssetBundle = _source.Load(by: _ident.AbsolutePathWithoutExtension);
			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Ident.Uri));

			_loadedAssetBundle = loadedAssetBundle;
			Size = _source.Size(of: _loadedAssetBundle);

			return _loadedAssetBundle;
		}

		public override async UniTask<AssetBundle> LoadAsync(DownloadProgressDelegate onProgress = null,
			CancellationToken cancellationToken = default)
		{
			if (IsLoaded)
			{
				onProgress?.Invoke(DownloadProgress.Full);
				return _loadedAssetBundle;
			}

			var loadedAssetBundle = await _source.LoadAsync(by: _ident.AbsolutePathWithoutExtension,
				with: Progress.Create<float>(value => onProgress?.Invoke(new DownloadProgress(value))),
				cancellationToken);

			Guard.AgainstNull(loadedAssetBundle, () => new AssetBundleNotLoadedException(Ident.Uri));

			_loadedAssetBundle = loadedAssetBundle;
			onProgress?.Invoke(DownloadProgress.Full);
			Size = _source.Size(of: _loadedAssetBundle);

			return _loadedAssetBundle;
		}

		public override void Unload()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_loadedAssetBundle.Unload(true);
			_loadedAssetBundle = null;
		}

		public void UnloadAsync()
		{
			if (IsLoaded == false)
			{
				return;
			}

			_loadedAssetBundle.UnloadAsync(true);
			_loadedAssetBundle = null;
		}

		void IDisposable.Dispose() => Unload();
	}
}