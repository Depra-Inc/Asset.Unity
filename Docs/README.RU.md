# Depra.Assets

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Оглавление</summary>

- [Введение](#-введение)
    - [Особенности](#-особенности)
    - [Возможности](#-возможности)
- [Установка](#-установка)
- [Содержание](#-содержание)
- [Примеры использования](#-примеры-использования)
    - [Загрузка ассета из ресурсов](#загрузка-ассета-из-ресурсов)
    - [Загрузка AssetBundle](#загрузка-assetbundle)
    - [Загрузка ассета из AssetBundle](#загрузка-ассета-из-assetbundle)
    - [Загрузка ассета из редакторской базы данных](#загрузка-ассета-из-редакторской-базы-данных)
    - [Загрузка ассета из настроек проекта](#загрузка-ассета-из-настроек-проекта)
- [Зависимости](#-зависимости)
- [Сотрудничество](#-сотрудничество)
- [Поддержка](#-поддержка)
- [Лицензия](#-лицензия)

</details>

## 🧾 Введение

Эта библиотека предоставляет классы и интерфейсы для удобной и эффективной загрузки различных типов ресурсов в **Unity**
проектах.

Она содержит общие методы и функциональность для работы с ассетами, а также реализации специфичных стратегий
загрузки для различных источников.

### 💡 Особенности:

- Однородный **API** для загрузки ассетов из различных источников;
- Поддержка отмены загрузки;
- Предоставление информации о прогрессе загрузки;
- Расширяемость.

### 🦾 Возможности:

| Возможность                                        | Runtime | Редакторе |
|----------------------------------------------------|---------|-----------|
| Загрузка ассетов из **Resources**                  | ✅       | ✅         |
| Загрузка **UnityEngine.AssetBundle**               | ✅       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetBundle**    | ✅       | ✅         |
| Загрузка ассетов из **UnityEditor.PlayerSettings** | ❌       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetDatabase**  | ❌       | ✅         |

## 📥 Установка

### 📦 Через **UPM**:

1. Откройте окно **Unity Package Manager**.
2. Нажмите на кнопку **+** в правом верхнем углу окна.
3. Выберите **Add package from git URL...**.
4. Введите [ссылку на репозиторий](https://github.com/Depra-Inc/Assets.Unity.git)
5. Нажмите **Add**.

### ⚙️ Ручная:

Добавьте в `Packages/manifest.json` в раздел `dependencies` следующую строку:

```json
"com.depra.assets.unity": "https://github.com/Depra-Inc/Assets.Unity.git"
```

## 📖 Содержание

`ILoadableAsset<TAsset>` - определяет базовые методы и свойства для загрузки и выгрузки ассетов.
Он расширяет интерфейс `IAssetFile` из [Depra.Assets](https://github.com/Depra-Inc/Assets) и
предоставляет возможности для синхронной и асинхронной загрузки, а также проверки состояния загрузки.

```csharp
public interface ILoadableAsset<TAsset> : IAssetFile
{
    bool IsLoaded { get; }
    
    TAsset Load();
    
    void Unload();
    
    Task<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null, CancellationToken cancellationToken = default);
}
```

| Тип класса ассета              | Назначение                                                                                                     |
|--------------------------------|----------------------------------------------------------------------------------------------------------------|
| `ResourceAsset<TAsset>`        | Загрузка и выгрузка ассетов из `UnityEngine.Resources`.                                                        |
| `AssetBundleFile`              | Загрузка и выгрузка `UnityEngine.AssetBundle`.                                                                 |
| `AssetBundleAssetFile<TAsset>` | Загрузка и выгрузка ассетов из `UnityEngine.AssetBundle`.                                                      |
| `DatabaseAsset<TAsset>`        | Загрузка и выгрузка ассетов из `UnityEditor.AssetDatabase`. ⚠️**Асинхронная загрузка пока не поддерживается.** |
| `PreloadedAsset<TAsset>`       | Загрузка и выгрузка ассетов из настроек проекта `UnityEditor.ProjectSettings`.                                 |

Все классы, реализующие интерфейс `ILoadableAsset`, также реализуют интерфейс `System.IDisposable`.
Добавлено для удобного использования в `using` блоках.

## 📋 Примеры использования

### Загрузка ассета из ресурсов

```csharp
var resourceTexture = new ResourceAsset<Texture2D>("Textures/myTexture");
Texture2D loadedTexture = resourceTexture.Load();
// Использование загруженного ассета.
resourceTexture.Unload();
```

### Загрузка AssetBundle

```csharp
var assetBundleFile = new AssetBundleFile("Path/To/MyBundle");
AssetBundle loadedBundle = assetBundleFile.Load();
// Использование загруженного ассета.
assetBundleFile.Unload();
```

### Загрузка ассета из AssetBundle

```csharp
var assetBundle = AssetBundle.LoadFromFile("Path/To/MyBundle");
var assetBundleAsset = new AssetBundleAssetFile<GameObject>("MyAsset", assetBundle);
GameObject loadedAsset = assetBundleAsset.Load();
// Использование загруженного ассета.
assetBundleAsset.Dispose();
```

### Загрузка ассета из редакторской базы данных

```csharp
var databaseAsset = new DatabaseAsset<MyScriptableObject>("Path/To/MyAsset");
MyScriptableObject loadedObject = databaseAsset.Load();
// Использование загруженного ассета.
databaseAsset.Dispose();
```

### Загрузка ассета из настроек проекта

```csharp
var preloadedAsset = new PreloadedAsset<GameObject>("Path/To/MyAsset");
GameObject loadedAsset = preloadedAsset.Load();
// Использование загруженного ассета.
preloadedAsset.Dispose();
```

## 🖇️ Зависимости

- [Depra.Assets](https://github.com/Depra-Inc/Assets) - базовая библиотека для работы с ассетами (
  поставляется вместе с этим **UPM** пакетом).

## 🤝 Сотрудничество

Я рад приветствовать запросы на добавление новых функций и сообщения об ошибках в
разделе [issues](https://github.com/Depra-Inc/Assets.Unity/issues) и также
принимать [pull requests](https://github.com/Depra-Inc/Assets.Unity/pulls).

## 🫂 Поддержка

Я независимый разработчик,
и большая часть разработки этого проекта выполняется в свободное время.
Если вы заинтересованы в сотрудничестве или найме меня для проекта,
ознакомьтесь с моим [портфолио](https://github.com/Depra-Inc)
и [свяжитесь со мной](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 Лицензия

Этот проект распространяется под лицензией
**[Apache-2.0](https://github.com/Depra-Inc/Assets.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Николай Мельников
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)