# Depra.Assets

<div>
    <strong><a href="README.md">English</a> | <a href="README.RU.md">Русский</a></strong>
</div>

<details>
<summary>Оглавление</summary>

- [Введение](#-введение)
    - [Особенности](#-особенности)
    - [Возможности](#-возможности)
- [Начало_работы](#-начало-работы)
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

## 🚀 Начало работы

Прежде чем начать использовать библиотеку **Depra.Assets** в вашем проекте **Unity**,
убедитесь, что проект соответсвует следующим условиям:

### Установите UniTask

**Depra.Assets** использует библиотеку **UniTask** для асинхронных операций.
Вы можете установить её следуя [этим инструкциям](https://github.com/Cysharp/UniTask#getting-started).

## 📥 Установка

### 📦 Через **UPM**:

1. Откройте окно **Unity Package Manager**.
2. Нажмите на кнопку **+** в правом верхнем углу окна.
3. Выберите **Add package from git URL...**.
4. Введите [ссылку на репозиторий](https://github.com/Depression-aggression/Assets.Unity.git)
5. Нажмите **Add**.

### ⚙️ Ручная:

Добавьте в `Packages/manifest.json` в раздел `dependencies` следующую строку:

```json
"com.depra.assets.unity": "https://github.com/Depression-aggression/Assets.Unity.git"
```

## 📖 Содержание

- `IUnityLoadableAsset<TAsset>` - определяет базовые методы и свойства для загрузки и выгрузки ассетов.
  Он расширяет интерфейс `IAssetFile` из [Depra.Assets](https://github.com/Depression-aggression/Assets) и
  предоставляет возможности для синхронной и асинхронной загрузки, а также проверки состояния загрузки.

```csharp
public interface IUnityLoadableAsset<TAsset> : IAssetFile
{
    bool IsLoaded { get; }
    
    TAsset Load();
    
    void Unload();
    
    UniTask<TAsset> LoadAsync(DownloadProgressDelegate onProgress = null,
        CancellationToken cancellationToken = default);
}
```

- `ResourceAsset<TAsset>` - предоставляет реализацию загрузки и выгрузки ассетов из ресурсов **Unity**.


- `AssetBundleFile` - предоставляет методы для загрузки и выгрузки `UnityEngine.AssetBundle`.


- `AssetBundleAssetFile<TAsset>` - обеспечивает загрузку и выгрузку ассетов из `UnityEngine.AssetBundle`.


- `DatabaseAsset<TAsset>` - позволяет загружать и выгружать ассеты из редакторской базы
  данных `UnityEditor.AssetDatabase`.


- `PreloadedAsset<TAsset>` - обеспечивает загрузку и выгрузку ассетов из настроек проекта `UnityEditor.ProjectSettings`.

Все классы, реализующие интерфейс `IUnityLoadableAsset`, также реализуют интерфейс `System.IDisposable`.
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

- [Depra.Assets](https://github.com/Depression-aggression/Assets) - базовая библиотека для работы с ассетами (
  поставляется вместе с этим UPM пакетом).
- [UniTask](https://github.com/Cysharp/UniTask) - библиотека для работы с асинхронными операциями.

## 🤝 Сотрудничество

Я рад приветствовать запросы на добавление новых функций и сообщения об ошибках в
разделе [issues](https://github.com/Depression-aggression/Assets.Unity/issues) и также
принимать [pull requests](https://github.com/Depression-aggression/Assets.Unity/pulls).

## 🫂 Поддержка

Я независимый разработчик,
и большая часть разработки этого проекта выполняется в свободное время.
Если вы заинтересованы в сотрудничестве или найме меня для проекта,
ознакомьтесь с моим [портфолио](https://github.com/Depression-aggression)
и [свяжитесь со мной](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 Лицензия

Этот проект распространяется под лицензией
**[Apache-2.0](https://github.com/Depression-aggression/Assets.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Николай Мельников
[g0dzZz1lla@yandex.ru](mailto:g0dzZz1lla@yandex.ru)