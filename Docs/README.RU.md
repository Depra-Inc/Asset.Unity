# Depra.Assets

![License](https://img.shields.io/github/license/Depra-Inc/Asset.Unity?style=flat-round)
![Last Commit](https://img.shields.io/github/last-commit/Depra-Inc/Asset.Unity?style=flat-round)
![Code Size](https://img.shields.io/github/languages/code-size/Depra-Inc/Asset.Unity?style=flat-round)

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

Эта библиотека предоставляет классы и интерфейсы для удобной и эффективной загрузки различных типов ассетов в **Unity**
проектах.

Она содержит общие методы и функциональность для работы с ассетами, а также реализации специфичных стратегий
загрузки для различных источников.

### 💡 Особенности:

- **Стандартизация**: Единое **API** для загрузки ассетов из различных источников.
- **Отмена загрузки**: Возможность отмены операции загрузки в любой момент.
- **Отслеживание прогресса**: Предоставление информации о текущем состоянии загрузки.
- **Расширяемость**: Гибкая архитектура для расширения функциональности по вашим потребностям.
- **Легковесность**: Пакет содержит всего лишь десятки килобайт необходимых скриптов и ничего лишнего.

Эти особенности делают библиотеку еще более мощной и удобной для ваших задач.

### 🦾 Возможности:

| Возможность                                        | Runtime | Редакторе |
|----------------------------------------------------|---------|-----------|
| Загрузка ассетов из **UnityEngine.Resources**      | ✅       | ✅         |
| Загрузка **UnityEngine.AssetBundle**               | ✅       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetBundle**    | ✅       | ✅         |
| Загрузка ассетов из **UnityEditor.PlayerSettings** | ❌       | ✅         |
| Загрузка ассетов из **UnityEngine.AssetDatabase**  | ❌       | ✅         |

## 📥 Установка

### 📦 Через **UPM**:

1. Откройте окно **Unity Package Manager**.
2. Нажмите на кнопку **+** в правом верхнем углу окна.
3. Выберите **Add package from git URL...**.
4. Введите [ссылку на репозиторий](https://github.com/Depra-Inc/Asset.Unity.git)
5. Нажмите **Add**.

### ⚙️ Через **UPM** используя `manifest.json`:

Добавьте в `Packages/manifest.json` в раздел `dependencies` следующую строку:

```
"com.depra.assets.unity": "https://github.com/Depra-Inc/Asset.Unity.git"
```

### 🛒 Через Unity Asset Store:

1. Откройте [Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/depra-assets-266429).
2. Добавьте пакет в ваши активы.
3. Нажмите **Open in Unity**.
4. Следуйте инструкциям в редакторе **Unity**.

## 📖 Содержание

**Ключевые концепции**, используемые в этой библиотеке, описаны в следующих интерфейсах:

- `IAssetUri`: Разработан для облегчения управления ресурсами в проектах **Unity**.
  Он предоставляет простой и унифицированный способ идентификации и управления ассетами с использованием **URI**
  *(Uniform Resource Identifier)*.

- `IAssetFile<TAsset>`: Определяет основные методы и свойства, необходимые для загрузки и выгрузки ассетов.
  Он расширяет функциональность интерфейса `IAssetFile`, представленного
  в [Depra.Assets](https://github.com/Depra-Inc/Asset), и предоставляет возможность выполнения как синхронной, так и
  асинхронной загрузки ассетов, а также проверки состояния загрузки.

Вы можете создать свои реализации этих интерфейсов или использовать уже готовые, представленные в таблице:

| Тип класса ассета              | Идентификатор      | Назначение                                                                                                     |
|--------------------------------|--------------------|----------------------------------------------------------------------------------------------------------------|
| `ResourcesAsset<TAsset>`       | `ResourcesPath`    | Загрузка и выгрузка ассетов из `UnityEngine.Resources`.                                                        |
| `AssetBundleFile`              | `AssetBundleUri`   | Загрузка и выгрузка `UnityEngine.AssetBundle`.                                                                 |
| `AssetBundleAssetFile<TAsset>` | `AssetName`        | Загрузка и выгрузка ассетов из `UnityEngine.AssetBundle`.                                                      |
| `EditorDatabaseAsset<TAsset>`  | `DatabaseAssetUri` | Загрузка и выгрузка ассетов из `UnityEditor.AssetDatabase`. ⚠️**Асинхронная загрузка пока не поддерживается.** |
| `PreloadedAsset<TAsset>`       | `IAssetUri`        | Загрузка и выгрузка ассетов из настроек проекта `UnityEditor.ProjectSettings`.                                 |

Все классы, реализующие интерфейс `IAssetFile<TAsset>`, также реализуют интерфейс `System.IDisposable`.
Добавлено для удобного использования в `using` блоках.

## 📋 Примеры использования

#### Загрузка ассета из ресурсов

```csharp
var resourceTexture = new ResourceAsset<Texture2D>("Textures/myTexture");
Texture2D loadedTexture = resourceTexture.Load();
// Использование загруженного ассета.
resourceTexture.Unload();
```

#### Загрузка AssetBundle

```csharp
var assetBundleSource = new AssetBundleFromFile();
var assetBundleFile = new AssetBundleFile("Path/To/MyBundle", assetBundleSource);
AssetBundle loadedBundle = assetBundleFile.Load();
// Использование загруженного ассета.
assetBundleFile.Unload();
```

#### Загрузка ассета из AssetBundle

```csharp
var assetBundle = AssetBundle.LoadFromFile("Path/To/MyBundle");
var assetBundleAsset = new AssetBundleAssetFile<GameObject>("MyAsset", assetBundle);
GameObject loadedAsset = assetBundleAsset.Load();
// Использование загруженного ассета.
assetBundleAsset.Unload();
```

#### Загрузка ассета из редакторской базы данных

```csharp
var databaseAsset = new EditorDatabaseAsset<MyScriptableObject>("Path/To/MyAsset");
MyScriptableObject loadedObject = databaseAsset.Load();
// Использование загруженного ассета.
databaseAsset.Unload();
```

#### Загрузка ассета из настроек проекта

```csharp
var anyAsset = new ResourcesAsset<GameObject>("Path/To/MyAsset");
var preloadedAsset = new PreloadedAsset<GameObject>(anyAsset);
GameObject loadedAsset = preloadedAsset.Load();
// Использование загруженного ассета.
preloadedAsset.Unload();
```

## 🖇️ Зависимости

- [Depra.Assets](https://github.com/Depra-Inc/Asset) - базовая библиотека для работы с ассетами (
  поставляется вместе с этим **UPM** пакетом).

## 🤝 Сотрудничество

Я рад приветствовать запросы на добавление новых функций и сообщения об ошибках в
разделе [issues](https://github.com/Depra-Inc/Asset.Unity/issues) и также
принимать [pull requests](https://github.com/Depra-Inc/Asset.Unity/pulls).

## 🫂 Поддержка

Я независимый разработчик, и большая часть разработки этого проекта выполняется в свободное время.
Если вы заинтересованы в сотрудничестве или найме меня для проекта,
ознакомьтесь с моим [портфолио](https://github.com/Depra-Inc)
и [свяжитесь со мной](mailto:g0dzZz1lla@yandex.ru)!

## 🔐 Лицензия

Этот проект распространяется под лицензией
**[Apache-2.0](https://github.com/Depra-Inc/Asset.Unity/blob/main/LICENSE.md)**

Copyright (c) 2023 Николай Мельников
[n.melnikov@depra.org](mailto:n.melnikov@depra.org)