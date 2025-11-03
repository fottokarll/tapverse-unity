using System.Collections.Generic;
using TapVerse.Core;
using TapVerse.Gameplay;
using TapVerse.Gameplay.Definitions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TapVerse.UI
{
    public class GameHUD : MonoBehaviour
    {
        private Text _currencyText;
        private Text _lifetimeText;
        private Text _shardsText;
        private Text _prestigePreviewText;
        private Button _prestigeButton;
        private GameObject _upgradesPanel;
        private GameObject _generatorsPanel;
        private GameObject _prestigePanel;
        private GameObject _settingsPanel;
        private readonly Dictionary<string, Button> _upgradeButtons = new Dictionary<string, Button>();
        private readonly Dictionary<string, Button> _generatorButtons = new Dictionary<string, Button>();
        private PrestigeManager _prestigeManager;
        private UpgradeManager _upgradeManager;
        private GeneratorManager _generatorManager;
        private CurrencyManager _currencyManager;
        private SaveManager _saveManager;

        private void Start()
        {
            CacheManagers();
            BuildUI();
            SubscribeEvents();
            RefreshCurrency();
            RefreshUpgradeList();
            RefreshGeneratorList();
            RefreshPrestige();
        }

        private void CacheManagers()
        {
            _currencyManager = ServiceLocator.Resolve<CurrencyManager>();
            _upgradeManager = ServiceLocator.Resolve<UpgradeManager>();
            _generatorManager = ServiceLocator.Resolve<GeneratorManager>();
            _prestigeManager = ServiceLocator.Resolve<PrestigeManager>();
            _saveManager = ServiceLocator.Resolve<SaveManager>();
        }

        private void SubscribeEvents()
        {
            GameEvents.CurrencyChanged += OnCurrencyChanged;
            GameEvents.LifetimeCurrencyChanged += OnCurrencyChanged;
            GameEvents.CreationShardsChanged += OnCurrencyChanged;
            GameEvents.UpgradesChanged += RefreshUpgradeList;
            GameEvents.GeneratorsChanged += RefreshGeneratorList;
            GameEvents.PrestigeAvailable += RefreshPrestige;
        }

        private void OnDestroy()
        {
            GameEvents.CurrencyChanged -= OnCurrencyChanged;
            GameEvents.LifetimeCurrencyChanged -= OnCurrencyChanged;
            GameEvents.CreationShardsChanged -= OnCurrencyChanged;
            GameEvents.UpgradesChanged -= RefreshUpgradeList;
            GameEvents.GeneratorsChanged -= RefreshGeneratorList;
            GameEvents.PrestigeAvailable -= RefreshPrestige;
        }

        private void OnCurrencyChanged(BigDouble _) => RefreshCurrency();

        private void BuildUI()
        {
            EnsureEventSystem();
            var canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.AddComponent<GraphicRaycaster>();

            var font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // Top bar
            var topBar = CreatePanel(canvasGO.transform, "TopBar", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -40f));
            var topLayout = topBar.AddComponent<HorizontalLayoutGroup>();
            topLayout.childForceExpandWidth = false;
            topLayout.spacing = 16f;
            _currencyText = CreateLabel(topBar.transform, font, "CE: 0");
            _lifetimeText = CreateLabel(topBar.transform, font, "Lifetime: 0");
            _shardsText = CreateLabel(topBar.transform, font, "CS: 0");

            // Universe core
            var coreGO = new GameObject("UniverseCore");
            coreGO.transform.SetParent(canvasGO.transform);
            var coreRect = coreGO.AddComponent<RectTransform>();
            coreRect.sizeDelta = new Vector2(300, 300);
            coreRect.anchorMin = new Vector2(0.5f, 0.5f);
            coreRect.anchorMax = new Vector2(0.5f, 0.5f);
            coreRect.anchoredPosition = Vector2.zero;
            var image = coreGO.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Art/Placeholders/universe_core");
            image.color = new Color(1f, 1f, 1f, 0.9f);
            coreGO.AddComponent<TapInputHandler>();

            // Bottom tab area
            var tabRoot = CreatePanel(canvasGO.transform, "TabRoot", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 120f));
            var tabLayout = tabRoot.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 12f;
            var upgradesTab = CreateTabButton(tabRoot.transform, font, "Upgrades", () => ShowPanel(_upgradesPanel));
            var generatorsTab = CreateTabButton(tabRoot.transform, font, "Generators", () => ShowPanel(_generatorsPanel));
            var prestigeTab = CreateTabButton(tabRoot.transform, font, "Prestige", () => ShowPanel(_prestigePanel));
            var settingsTab = CreateTabButton(tabRoot.transform, font, "Settings", () => ShowPanel(_settingsPanel));

            // Panels container
            var panelsRoot = CreatePanel(canvasGO.transform, "Panels", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 260f));
            var panelsRect = panelsRoot.GetComponent<RectTransform>();
            panelsRect.sizeDelta = new Vector2(0, 400f);

            _upgradesPanel = CreateContentPanel(panelsRoot.transform, "UpgradesPanel");
            _generatorsPanel = CreateContentPanel(panelsRoot.transform, "GeneratorsPanel");
            _prestigePanel = CreateContentPanel(panelsRoot.transform, "PrestigePanel");
            _settingsPanel = CreateContentPanel(panelsRoot.transform, "SettingsPanel");

            BuildUpgradesPanel(_upgradesPanel.transform, font);
            BuildGeneratorsPanel(_generatorsPanel.transform, font);
            BuildPrestigePanel(_prestigePanel.transform, font);
            BuildSettingsPanel(_settingsPanel.transform, font);

            ShowPanel(_upgradesPanel);
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                var eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(0, 60f);
            var image = go.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.35f);
            return go;
        }

        private Text CreateLabel(Transform parent, Font font, string text)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent);
            var label = go.AddComponent<Text>();
            label.font = font;
            label.fontSize = 20;
            label.color = Color.white;
            label.text = text;
            return label;
        }

        private Button CreateTabButton(Transform parent, Font font, string label, System.Action onClick)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(150, 60);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.15f, 0.2f, 0.35f, 0.8f);
            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => onClick?.Invoke());
            var text = CreateLabel(go.transform, font, label);
            text.alignment = TextAnchor.MiddleCenter;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            return button;
        }

        private GameObject CreateContentPanel(Transform parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(700f, 300f);
            go.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.4f);
            return go;
        }

        private void BuildUpgradesPanel(Transform parent, Font font)
        {
            var scroll = CreateScrollView(parent, font, out Transform content);
            var catalog = ServiceLocator.Resolve<GameManager>().UpgradeCatalog;
            foreach (var upgrade in catalog.Upgrades)
            {
                var button = CreateListButton(content, font, upgrade.DisplayName, () => OnUpgradePressed(upgrade));
                _upgradeButtons[upgrade.Id] = button;
            }
        }

        private void BuildGeneratorsPanel(Transform parent, Font font)
        {
            var scroll = CreateScrollView(parent, font, out Transform content);
            var catalog = ServiceLocator.Resolve<GameManager>().GeneratorCatalog;
            foreach (var generator in catalog.Generators)
            {
                var button = CreateListButton(content, font, generator.DisplayName, () => OnGeneratorPressed(generator));
                _generatorButtons[generator.Id] = button;
            }
        }

        private void BuildPrestigePanel(Transform parent, Font font)
        {
            _prestigePreviewText = CreateLabel(parent, font, "Prestige grants 0 CS");
            _prestigePreviewText.alignment = TextAnchor.MiddleCenter;
            var button = CreateListButton(parent, font, "Begin Big Bang Reborn", OnPrestigePressed);
            _prestigeButton = button;
        }

        private void BuildSettingsPanel(Transform parent, Font font)
        {
            var column = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            column.spacing = 12f;
            column.padding = new RectOffset(16, 16, 16, 16);
            CreateToggle(parent, font, "SFX", value => _saveManager.Data.Options.SfxEnabled = value, _saveManager.Data.Options.SfxEnabled);
            CreateToggle(parent, font, "Haptics", value => _saveManager.Data.Options.HapticsEnabled = value, _saveManager.Data.Options.HapticsEnabled);
            CreateToggle(parent, font, "Reduced Motion", value => _saveManager.Data.Options.ReducedMotion = value, _saveManager.Data.Options.ReducedMotion);
        }

        private GameObject CreateScrollView(Transform parent, Font font, out Transform content)
        {
            var scrollGO = new GameObject("ScrollView");
            scrollGO.transform.SetParent(parent);
            var rect = scrollGO.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(10, 10);
            rect.offsetMax = new Vector2(-10, -10);
            var image = scrollGO.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.2f);
            var scroll = scrollGO.AddComponent<ScrollRect>();
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollGO.transform);
            var viewportRect = viewport.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            viewport.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.1f);
            var contentGO = new GameObject("Content");
            contentGO.transform.SetParent(viewport.transform);
            var contentRect = contentGO.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = Vector2.zero;
            var layout = contentGO.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 8f;
            layout.padding = new RectOffset(12, 12, 12, 12);
            contentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewportRect;
            scroll.content = contentRect;
            content = contentRect;
            return scrollGO;
        }

        private Button CreateListButton(Transform parent, Font font, string label, System.Action onClick)
        {
            var go = new GameObject(label + "Entry");
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 60f);
            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.25f, 0.35f, 0.8f);
            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() => onClick?.Invoke());
            var text = CreateLabel(go.transform, font, label);
            text.alignment = TextAnchor.MiddleLeft;
            var textRect = text.rectTransform;
            textRect.anchorMin = new Vector2(0f, 0f);
            textRect.anchorMax = new Vector2(1f, 1f);
            textRect.offsetMin = new Vector2(16, 8);
            textRect.offsetMax = new Vector2(-16, -8);
            return button;
        }

        private void CreateToggle(Transform parent, Font font, string label, UnityEngine.Events.UnityAction<bool> onValueChanged, bool initial)
        {
            var go = new GameObject(label + "Toggle");
            go.transform.SetParent(parent);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 40f);
            var toggle = go.AddComponent<Toggle>();
            var background = new GameObject("Background");
            background.transform.SetParent(go.transform);
            var bgRect = background.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.5f);
            bgRect.anchorMax = new Vector2(0f, 0.5f);
            bgRect.sizeDelta = new Vector2(32f, 32f);
            bgRect.anchoredPosition = new Vector2(16f, 0f);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform);
            var checkRect = checkmark.AddComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.5f, 0.5f);
            checkRect.anchorMax = new Vector2(0.5f, 0.5f);
            checkRect.sizeDelta = new Vector2(20f, 20f);
            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.8f, 0.3f, 1f);
            toggle.graphic = checkImage;
            toggle.targetGraphic = bgImage;
            toggle.isOn = initial;
            toggle.onValueChanged.AddListener(value =>
            {
                onValueChanged.Invoke(value);
                _saveManager.AutoSave();
            });
            var labelText = CreateLabel(go.transform, font, label);
            labelText.alignment = TextAnchor.MiddleLeft;
            labelText.rectTransform.anchorMin = new Vector2(0, 0);
            labelText.rectTransform.anchorMax = new Vector2(1, 1);
            labelText.rectTransform.offsetMin = new Vector2(60, 0);
            labelText.rectTransform.offsetMax = new Vector2(0, 0);
        }

        private void ShowPanel(GameObject panel)
        {
            _upgradesPanel.SetActive(panel == _upgradesPanel);
            _generatorsPanel.SetActive(panel == _generatorsPanel);
            _prestigePanel.SetActive(panel == _prestigePanel);
            _settingsPanel.SetActive(panel == _settingsPanel);
        }

        private void RefreshCurrency()
        {
            _currencyText.text = $"CE: {_currencyManager.Current.ToShortString()}";
            _lifetimeText.text = $"Lifetime: {_currencyManager.Lifetime.ToShortString()}";
            _shardsText.text = $"CS: {_currencyManager.CreationShards.ToShortString()}";
            RefreshPrestige();
        }

        private void RefreshUpgradeList()
        {
            var catalog = ServiceLocator.Resolve<GameManager>().UpgradeCatalog;
            foreach (var upgrade in catalog.Upgrades)
            {
                if (_upgradeButtons.TryGetValue(upgrade.Id, out var button))
                {
                    int level = _upgradeManager.GetLevel(upgrade.Id);
                    var cost = _upgradeManager.GetCost(upgrade);
                    var text = button.GetComponentInChildren<Text>();
                    text.text = $"{upgrade.DisplayName} Lv.{level} - {cost.ToShortString()}";
                    button.interactable = _upgradeManager.CanPurchase(upgrade) && _currencyManager.Current >= cost;
                }
            }
        }

        private void RefreshGeneratorList()
        {
            var catalog = ServiceLocator.Resolve<GameManager>().GeneratorCatalog;
            foreach (var generator in catalog.Generators)
            {
                if (_generatorButtons.TryGetValue(generator.Id, out var button))
                {
                    int count = _generatorManager.GetCount(generator.Id);
                    var cost = _generatorManager.GetCost(generator);
                    var text = button.GetComponentInChildren<Text>();
                    text.text = $"{generator.DisplayName} x{count} - {cost.ToShortString()}";
                    button.interactable = _currencyManager.Current >= cost;
                }
            }
        }

        private void RefreshPrestige()
        {
            if (_prestigeManager == null) return;
            var preview = _prestigeManager.GetPreview();
            if (_prestigePreviewText != null)
            {
                _prestigePreviewText.text = $"Prestige grants +{preview.AdditionalShards.ToShortString()} CS";
            }

            if (_prestigeButton != null)
            {
                _prestigeButton.interactable = _prestigeManager.CanPrestige();
            }
        }

        private void OnUpgradePressed(UpgradeDefinition upgrade)
        {
            _upgradeManager.TryPurchase(upgrade);
            RefreshUpgradeList();
        }

        private void OnGeneratorPressed(GeneratorDefinition generator)
        {
            _generatorManager.TryPurchase(generator);
            RefreshGeneratorList();
        }

        private void OnPrestigePressed()
        {
            _prestigeManager.ConfirmPrestige();
            RefreshPrestige();
        }
    }
}
