#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using App.Boot;
using App.Game;
using App.Login;
using Framework.Patch;

public static class SceneGenerator
{
    private const string ScenesRoot = "Assets/Scenes";
    private const string BootScenePath = ScenesRoot + "/Boot.unity";
    private const string LoginScenePath = ScenesRoot + "/Login.unity";
    private const string GameScenePath = ScenesRoot + "/Game.unity";

    [MenuItem("Tools/Generate SlotGame Scenes")]
    public static void GenerateScenes()
    {
        if (!AssetDatabase.IsValidFolder(ScenesRoot))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        GenerateBootScene();
        GenerateLoginScene();
        GenerateGameScene();

        AddScenesToBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("SlotGame scenes generated and added to Build Settings.");
    }

    private static void GenerateBootScene()
    {
        if (File.Exists(BootScenePath))
        {
            Debug.Log($"Scene already exists: {BootScenePath}");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = Path.GetFileNameWithoutExtension(BootScenePath);

        var root = new GameObject("AppBoot");
        root.AddComponent<AppBoot>();
        root.AddComponent<PatchBoot>();

        EditorSceneManager.SaveScene(scene, BootScenePath);
    }

    private static void GenerateLoginScene()
    {
        if (File.Exists(LoginScenePath))
        {
            Debug.Log($"Scene already exists: {LoginScenePath}");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = Path.GetFileNameWithoutExtension(LoginScenePath);

        CreateEventSystem();
        CreateCamera();

        var root = new GameObject("LoginScene");
        var controller = root.AddComponent<LoginController>();

        var canvas = CreateCanvas("LoginCanvas");
        canvas.transform.SetParent(root.transform, false);

        var viewGO = CreateUIElement("LoginView", canvas.transform);
        viewGO.AddComponent<LoginView>();

        CreateText(viewGO.transform, "Title", "Login", 34, new Vector2(0.5f, 0.8f));
        var inputGO = CreateInputField(viewGO.transform, "PlayerNameInput", "Player Name", new Vector2(0.5f, 0.55f));
        CreateText(viewGO.transform, "MessageText", "Enter your player name and press Login.", 20, new Vector2(0.5f, 0.35f));
        var buttonGO = CreateButton(viewGO.transform, "LoginButton", "Login", new Vector2(0.5f, 0.2f));

        var view = viewGO.GetComponent<LoginView>();
        viewGO.AddComponent<CanvasRenderer>();

        var button = buttonGO.GetComponent<Button>();
        UnityEventTools.AddPersistentListener(button.onClick, controller.OnLoginButtonClicked);

        EditorSceneManager.SaveScene(scene, LoginScenePath);
    }

    private static void GenerateGameScene()
    {
        if (File.Exists(GameScenePath))
        {
            Debug.Log($"Scene already exists: {GameScenePath}");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = Path.GetFileNameWithoutExtension(GameScenePath);

        CreateEventSystem();
        CreateCamera();

        var root = new GameObject("GameScene");
        root.AddComponent<GameController>();

        var canvas = CreateCanvas("GameCanvas");
        canvas.transform.SetParent(root.transform, false);

        var viewGO = CreateUIElement("GameView", canvas.transform);
        viewGO.AddComponent<GameView>();
        CreateText(viewGO.transform, "StatusText", "Game is running...", 26, new Vector2(0.5f, 0.5f));

        EditorSceneManager.SaveScene(scene, GameScenePath);
    }

    private static GameObject CreateCanvas(string name)
    {
        var canvasGO = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        return canvasGO;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
            return;

        var eventSystemGO = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystemGO.hideFlags = HideFlags.HideAndDontSave;
    }

    private static void CreateCamera()
    {
        var cameraGO = new GameObject("Main Camera");
        var camera = cameraGO.AddComponent<Camera>();
        camera.tag = "MainCamera";
        cameraGO.AddComponent<AudioListener>();
    }

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return go;
    }

    private static void CreateText(Transform parent, string name, string text, int fontSize, Vector2 anchor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(800, 80);
        rect.anchoredPosition = Vector2.zero;

        var uiText = go.GetComponent<Text>();
        uiText.text = text;
        uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        uiText.fontSize = fontSize;
        uiText.color = Color.white;
        uiText.alignment = TextAnchor.MiddleCenter;
    }

    private static GameObject CreateButton(Transform parent, string name, string text, Vector2 anchor)
    {
        var buttonGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonGO.transform.SetParent(parent, false);
        var rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(300, 80);
        rect.anchoredPosition = Vector2.zero;

        var buttonImage = buttonGO.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.5f, 0.9f);

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGO.transform.SetParent(buttonGO.transform, false);
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(1, 1);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var uiText = textGO.GetComponent<Text>();
        uiText.text = text;
        uiText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        uiText.fontSize = 24;
        uiText.color = Color.white;
        uiText.alignment = TextAnchor.MiddleCenter;

        return buttonGO;
    }

    private static GameObject CreateInputField(Transform parent, string name, string placeholder, Vector2 anchor)
    {
        var inputRoot = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField));
        inputRoot.transform.SetParent(parent, false);
        var rect = inputRoot.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(600, 80);
        rect.anchoredPosition = Vector2.zero;

        var image = inputRoot.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.9f);

        var placeholderGO = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        placeholderGO.transform.SetParent(inputRoot.transform, false);
        var placeholderRect = placeholderGO.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(10, 10);
        placeholderRect.offsetMax = new Vector2(-10, -10);

        var placeholderText = placeholderGO.GetComponent<Text>();
        placeholderText.text = placeholder;
        placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        placeholderText.fontSize = 22;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        placeholderText.alignment = TextAnchor.MiddleLeft;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGO.transform.SetParent(inputRoot.transform, false);
        var textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        var inputText = textGO.GetComponent<Text>();
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputText.fontSize = 22;
        inputText.color = Color.black;
        inputText.alignment = TextAnchor.MiddleLeft;

        var inputField = inputRoot.GetComponent<InputField>();
        inputField.textComponent = inputText;
        inputField.placeholder = placeholderText;

        return inputRoot;
    }

    private static GameObject CreateSlider(Transform parent, string name, float value, Vector2 anchor)
    {
        var sliderGO = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Slider));
        sliderGO.transform.SetParent(parent, false);
        var rect = sliderGO.GetComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.sizeDelta = new Vector2(600, 40);
        rect.anchoredPosition = Vector2.zero;

        var backgroundGO = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        backgroundGO.transform.SetParent(sliderGO.transform, false);
        var bgRect = backgroundGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = backgroundGO.GetComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        var fillAreaGO = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        var fillAreaRect = fillAreaGO.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1, 0.75f);
        fillAreaRect.offsetMin = new Vector2(10, 0);
        fillAreaRect.offsetMax = new Vector2(-10, 0);

        var fillGO = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        var fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        var fillImage = fillGO.GetComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f, 1f);

        var handleSlideAreaGO = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleSlideAreaGO.transform.SetParent(sliderGO.transform, false);
        var handleAreaRect = handleSlideAreaGO.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = Vector2.zero;
        handleAreaRect.offsetMax = Vector2.zero;

        var handleGO = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handleGO.transform.SetParent(handleSlideAreaGO.transform, false);
        var handleRect = handleGO.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 40);
        var handleImage = handleGO.GetComponent<Image>();
        handleImage.color = Color.white;

        var slider = sliderGO.GetComponent<Slider>();
        slider.targetGraphic = handleImage;
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.direction = Slider.Direction.LeftToRight;
        slider.value = value;

        return sliderGO;
    }

    private static void AddScenesToBuildSettings()
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();

        if (File.Exists(BootScenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(BootScenePath, true));
        }

        AddSceneIfExists(scenes, LoginScenePath);
        AddSceneIfExists(scenes, GameScenePath);

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void AddSceneIfExists(System.Collections.Generic.List<EditorBuildSettingsScene> scenes, string path)
    {
        if (File.Exists(path))
        {
            scenes.Add(new EditorBuildSettingsScene(path, true));
        }
    }
}
#endif
