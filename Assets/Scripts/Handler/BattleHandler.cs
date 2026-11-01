using Assets.Scripts.Battle;
using Assets.Scripts.Controller;
using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameData;

// Battle screen. Builds its whole uGUI hierarchy from code (no prefab / scene setup needed)
// and is keyboard only: W/S/A/D or arrows = move, E = confirm / timing hit, X = back.
public class BattleUI : MonoBehaviour, IBattleMenu
{
    private class UnitView
    {
        public BattleUnit Unit;
        public RectTransform Root;
        public Image Body, Highlight, HpFill, MpFill;
        public Text NameText, StatText, PopupText;
        public Coroutine PopupRoutine;
    }

    private static readonly Color PlayerColor = new Color(0.35f, 0.65f, 1f);
    private static readonly Color EnemyColor = new Color(1f, 0.45f, 0.4f);

    private BattleController _battle;
    private readonly Dictionary<BattleUnit, UnitView> _views = new();
    private BattleUnit _currentActor;

    private Font _font;
    private RectTransform _root;
    private Text _messageText, _menuText, _hintText, _timingLabel, _ratingText, _resultText;
    private GameObject _menuPanel, _timingPanel, _resultPanel;
    private RectTransform _goodZone, _perfectZone, _cursor;
    private Image _marker;
    private Coroutine _ratingRoutine;


    // ------------------------------------------------------------ setup

    private void Awake()
    {
        BuildUI();
    }

    public void Bind(BattleController battle)
    {
        _battle = battle;
        _battle.Menu = this;
        _battle.OnBattleStart += HandleBattleStart;
        _battle.OnTurnStart += HandleTurnStart;
        _battle.OnSkillUsed += HandleSkillUsed;
        _battle.OnDamaged += HandleDamaged;
        _battle.OnHealed += HandleHealed;
        _battle.OnCommandBegin += HandleCommandBegin;
        _battle.OnCommandEnd += HandleCommandEnd;
        _battle.OnBattleEnd += HandleBattleEnd;
    }

    private void OnDestroy()
    {
        if (_battle == null) { return; }
        _battle.OnBattleStart -= HandleBattleStart;
        _battle.OnTurnStart -= HandleTurnStart;
        _battle.OnSkillUsed -= HandleSkillUsed;
        _battle.OnDamaged -= HandleDamaged;
        _battle.OnHealed -= HandleHealed;
        _battle.OnCommandBegin -= HandleCommandBegin;
        _battle.OnCommandEnd -= HandleCommandEnd;
        _battle.OnBattleEnd -= HandleBattleEnd;
    }

    private void BuildUI()
    {
        _font = Font.CreateDynamicFontFromOSFont(new[] { "Arial", "Helvetica", "Liberation Sans" }, 16);

        var canvasGo = new GameObject("BattleCanvas", typeof(Canvas), typeof(CanvasScaler));
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        _root = (RectTransform)canvasGo.transform;

        MakeImage(_root, "Background", new Color(0.07f, 0.08f, 0.13f, 1f), V(0, 0), V(1, 1));

        _messageText = MakeText(_root, "Message", 28, TextAnchor.MiddleCenter, Color.white, V(0.1f, 0.88f), V(0.9f, 0.98f));
        MakeText(_root, "FleeHint", 16, TextAnchor.LowerLeft, Color.gray, V(0.01f, 0.01f), V(0.25f, 0.06f)).text = "Esc: flee";

        // command menu (bottom center)
        _menuPanel = MakeImage(_root, "MenuPanel", new Color(0f, 0f, 0f, 0.75f), V(0.3f, 0.03f), V(0.7f, 0.30f)).gameObject;
        _menuText = MakeText(_menuPanel.transform, "MenuText", 24, TextAnchor.UpperLeft, Color.white, V(0.05f, 0.2f), V(0.95f, 0.95f));
        _hintText = MakeText(_menuPanel.transform, "Hint", 16, TextAnchor.LowerCenter, Color.gray, V(0f, 0f), V(1f, 0.2f));
        _menuPanel.SetActive(false);

        // timing bar (above the menu)
        var timing = MakeImage(_root, "TimingPanel", new Color(0f, 0f, 0f, 0.8f), V(0.25f, 0.34f), V(0.75f, 0.47f));
        _timingPanel = timing.gameObject;
        _timingLabel = MakeText(_timingPanel.transform, "Label", 20, TextAnchor.UpperCenter, Color.white, V(0f, 0.6f), V(1f, 1f));
        var track = MakeImage(_timingPanel.transform, "Track", new Color(0.25f, 0.25f, 0.3f, 1f), V(0.04f, 0.12f), V(0.96f, 0.55f));
        _goodZone = MakeImage(track.transform, "Good", new Color(0.2f, 0.7f, 0.3f, 1f), V(0.25f, 0f), V(0.75f, 1f)).rectTransform;
        _perfectZone = MakeImage(track.transform, "Perfect", new Color(1f, 0.85f, 0.2f, 1f), V(0.42f, 0f), V(0.58f, 1f)).rectTransform;
        _cursor = MakeImage(track.transform, "Cursor", Color.white, V(-0.01f, -0.15f), V(0.01f, 1.15f)).rectTransform;
        _timingPanel.SetActive(false);

        _ratingText = MakeText(_root, "Rating", 48, TextAnchor.MiddleCenter, Color.white, V(0.25f, 0.34f), V(0.75f, 0.47f));
        _ratingText.gameObject.SetActive(false);

        // target marker (re-parented under the unit being targeted)
        _marker = MakeImage(_root, "TargetMarker", new Color(1f, 0.9f, 0.2f, 1f), V(0.46f, 0.78f), V(0.54f, 0.86f));
        _marker.gameObject.SetActive(false);

        // result overlay
        var result = MakeImage(_root, "ResultPanel", new Color(0f, 0f, 0f, 0.8f), V(0f, 0f), V(1f, 1f));
        _resultPanel = result.gameObject;
        _resultText = MakeText(_resultPanel.transform, "ResultText", 80, TextAnchor.MiddleCenter, Color.white, V(0f, 0.45f), V(1f, 0.7f));
        MakeText(_resultPanel.transform, "ResultHint", 26, TextAnchor.MiddleCenter, Color.gray, V(0f, 0.3f), V(1f, 0.42f)).text = "Press E to continue";
        _resultPanel.SetActive(false);
    }

    private static Vector2 V(float x, float y) { return new Vector2(x, y); }

    private Image MakeImage(Transform parent, string name, Color color, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private Text MakeText(Transform parent, string name, int size, TextAnchor align, Color color, Vector2 aMin, Vector2 aMax)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = (RectTransform)go.transform;
        rt.anchorMin = aMin;
        rt.anchorMax = aMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var text = go.GetComponent<Text>();
        text.font = _font;
        text.fontSize = size;
        text.alignment = align;
        text.color = color;
        text.supportRichText = true;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.raycastTarget = false;
        return text;
    }


    // ------------------------------------------------------------ unit views

    private void HandleBattleStart(IReadOnlyList<BattleUnit> units)
    {
        var players = new List<BattleUnit>();
        var enemies = new List<BattleUnit>();
        foreach (var u in units)
            (u.Team == Team.Player ? players : enemies).Add(u);

        CreateColumn(players, 0.22f);
        CreateColumn(enemies, 0.78f);
        SetMessage("Battle start!");
    }

    private void CreateColumn(List<BattleUnit> list, float x)
    {
        int n = list.Count;
        for (int i = 0; i < n; i++)
        {
            float y = n == 1 ? 0.6f : Mathf.Lerp(0.78f, 0.42f, i / (n - 1f));
            _views[list[i]] = CreateUnitView(list[i], x, y, n > 3 ? 0.75f : 1f);
        }
    }

    private UnitView CreateUnitView(BattleUnit unit, float x, float y, float scale)
    {
        var view = new UnitView { Unit = unit };

        var go = new GameObject("Unit_" + unit.DisplayName(), typeof(RectTransform));
        go.transform.SetParent(_root, false);
        view.Root = (RectTransform)go.transform;
        view.Root.anchorMin = view.Root.anchorMax = V(x, y);
        view.Root.sizeDelta = new Vector2(220f, 200f);
        view.Root.localScale = new Vector3(scale, scale, 1f);

        bool isPlayer = unit.Team == Team.Player;

        view.NameText = MakeText(view.Root, "Name", 22, TextAnchor.LowerCenter, Color.white, V(0f, 0.80f), V(1f, 1f));
        view.NameText.text = unit.DisplayName();

        view.Highlight = MakeImage(view.Root, "Highlight", new Color(1f, 0.9f, 0.2f, 0f), V(0.26f, 0.29f), V(0.74f, 0.81f));
        view.Body = MakeImage(view.Root, "Body", isPlayer ? PlayerColor : EnemyColor, V(0.30f, 0.32f), V(0.70f, 0.78f));
        if (unit.Data.CharacterSprite != null)
        {
            view.Body.sprite = unit.Data.CharacterSprite;
            view.Body.color = Color.white;
            view.Body.preserveAspect = true;
        }

        var hpBg = MakeImage(view.Root, "HpBg", new Color(0.15f, 0.15f, 0.15f, 1f), V(0.1f, 0.22f), V(0.9f, 0.30f));
        view.HpFill = MakeImage(hpBg.transform, "HpFill", new Color(0.3f, 0.85f, 0.35f, 1f), V(0f, 0f), V(1f, 1f));

        if (isPlayer)
        {
            var mpBg = MakeImage(view.Root, "MpBg", new Color(0.15f, 0.15f, 0.15f, 1f), V(0.1f, 0.14f), V(0.9f, 0.20f));
            view.MpFill = MakeImage(mpBg.transform, "MpFill", new Color(0.3f, 0.55f, 1f, 1f), V(0f, 0f), V(1f, 1f));
        }

        view.StatText = MakeText(view.Root, "Stat", 18, TextAnchor.UpperCenter, Color.white, V(0f, 0f), V(1f, 0.14f));

        view.PopupText = MakeText(view.Root, "Popup", 40, TextAnchor.MiddleCenter, Color.white, V(0f, 0.55f), V(1f, 0.95f));
        view.PopupText.gameObject.SetActive(false);
        return view;
    }

    private void Update()
    {
        foreach (var kv in _views)
            RefreshView(kv.Value);
    }

    private void RefreshView(UnitView v)
    {
        var u = v.Unit;
        SetFill(v.HpFill, (float)u.Hp / u.Data.MaxHp);
        string stat = "HP " + u.Hp + "/" + u.Data.MaxHp;
        if (v.MpFill != null)
        {
            SetFill(v.MpFill, (float)u.Mp / CharacterData.MaxMp);
            stat += "   MP " + u.Mp + "/" + CharacterData.MaxMp;
        }
        if (v.StatText.text != stat) v.StatText.text = stat;

        if (v.Unit.Data.CharacterSprite != null)
            v.Body.color = u.IsDead ? new Color(1f, 1f, 1f, 0.25f) : Color.white;
        else
        {
            Color c = u.Team == Team.Player ? PlayerColor : EnemyColor;
            v.Body.color = u.IsDead ? new Color(c.r, c.g, c.b, 0.25f) : c;
        }

        bool active = u == _currentActor && !u.IsDead;
        v.Highlight.color = new Color(1f, 0.9f, 0.2f, active ? 0.5f : 0f);
    }

    private static void SetFill(Image fill, float ratio)
    {
        fill.rectTransform.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
    }

    private void ShowPopup(BattleUnit unit, string text, Color color)
    {
        if (!_views.TryGetValue(unit, out var view)) { return; }
        if (view.PopupRoutine != null) StopCoroutine(view.PopupRoutine);
        view.PopupRoutine = StartCoroutine(PopupRoutine(view, text, color));
    }

    private IEnumerator PopupRoutine(UnitView v, string text, Color c)
    {
        const float duration = 0.9f;
        var rt = v.PopupText.rectTransform;
        v.PopupText.text = text;
        v.PopupText.gameObject.SetActive(true);
        float t = 0f;
        while (t < duration)
        {
            t += TimeUtils.GetFrameTimeSec();
            float k = Mathf.Clamp01(t / duration);
            rt.anchorMin = new Vector2(0f, 0.55f + 0.15f * k);
            rt.anchorMax = new Vector2(1f, 0.95f + 0.15f * k);
            v.PopupText.color = new Color(c.r, c.g, c.b, 1f - k);
            yield return null;
        }
        v.PopupText.gameObject.SetActive(false);
        v.PopupRoutine = null;
    }

    private void SetMessage(string msg)
    {
        _messageText.text = msg;
    }


    // ------------------------------------------------------------ battle events

    private void HandleTurnStart(BattleUnit actor)
    {
        _currentActor = actor;
        SetMessage(actor.DisplayName() + "'s turn");
    }

    private void HandleSkillUsed(BattleUnit actor, SkillData skill)
    {
        SetMessage(actor.DisplayName() + " uses " + skill.SkillName);
    }

    private void HandleDamaged(BattleUnit actor, BattleUnit target, int dealt, bool crit, CommandRanting rating)
    {
        bool byPlayer = actor.Team == Team.Player;
        string popup;
        Color color;

        if (dealt > 0)
        {
            popup = crit ? dealt + "!" : dealt.ToString();
            color = crit ? new Color(1f, 0.6f, 0.1f) : (byPlayer ? Color.white : new Color(1f, 0.4f, 0.4f));
            SetMessage(actor.DisplayName() + " hits " + target.DisplayName() + " for " + dealt + (crit ? " (CRIT!)" : ""));
        }
        else if (byPlayer && rating == CommandRanting.Miss)
        {
            popup = "MISS";
            color = Color.gray;
            SetMessage(actor.DisplayName() + " missed!");
        }
        else if (!byPlayer && rating == CommandRanting.Perfect)
        {
            popup = "PARRY";
            color = new Color(1f, 0.9f, 0.2f);
            SetMessage(target.DisplayName() + " parried " + actor.DisplayName() + "'s attack!");
        }
        else
        {
            popup = "0";
            color = Color.gray;
            SetMessage(actor.DisplayName() + "'s attack did no damage");
        }

        ShowPopup(target, popup, color);
    }

    private void HandleHealed(BattleUnit actor, BattleUnit target, int amount)
    {
        ShowPopup(target, "+" + amount, new Color(0.4f, 1f, 0.5f));
        SetMessage(actor.DisplayName() + " heals " + target.DisplayName() + " for " + amount);
    }

    private void HandleCommandBegin(IActionCommand cmd)
    {
        if (cmd is not TimingCommand timing) { return; }

        SetZone(_goodZone, timing.GoodWindow);
        SetZone(_perfectZone, timing.PerfectWindow);
        timing.OnProgress = SetCursor;
        SetCursor(0f);

        bool parry = _currentActor != null && _currentActor.Team == Team.Enemy;
        _timingLabel.text = parry ? "PARRY!  Press E at the center" : "Press E at the center";

        if (_ratingRoutine != null) { StopCoroutine(_ratingRoutine); _ratingRoutine = null; }
        _ratingText.gameObject.SetActive(false);
        _timingPanel.SetActive(true);
    }

    private static void SetZone(RectTransform zone, float halfWidth)
    {
        zone.anchorMin = new Vector2(Mathf.Clamp01(0.5f - halfWidth), 0f);
        zone.anchorMax = new Vector2(Mathf.Clamp01(0.5f + halfWidth), 1f);
    }

    private void SetCursor(float progress)
    {
        float p = Mathf.Clamp01(progress);
        _cursor.anchorMin = new Vector2(p - 0.01f, -0.15f);
        _cursor.anchorMax = new Vector2(p + 0.01f, 1.15f);
    }

    private void HandleCommandEnd(CommandRanting rating)
    {
        _timingPanel.SetActive(false);

        bool parry = _currentActor != null && _currentActor.Team == Team.Enemy;
        string label;
        Color color;
        switch (rating)
        {
            case CommandRanting.Perfect:
                label = parry ? "PARRIED!" : "PERFECT!";
                color = new Color(1f, 0.9f, 0.2f);
                break;
            case CommandRanting.Good:
                label = "GOOD";
                color = new Color(0.4f, 1f, 0.5f);
                break;
            default:
                label = "MISS";
                color = new Color(1f, 0.4f, 0.4f);
                break;
        }

        if (_ratingRoutine != null) StopCoroutine(_ratingRoutine);
        _ratingRoutine = StartCoroutine(RatingRoutine(label, color));
    }

    private IEnumerator RatingRoutine(string label, Color color)
    {
        _ratingText.text = label;
        _ratingText.color = color;
        _ratingText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        _ratingText.gameObject.SetActive(false);
        _ratingRoutine = null;
    }

    private void HandleBattleEnd(Team winner)
    {
        StartCoroutine(ResultRoutine(winner));
    }

    private IEnumerator ResultRoutine(Team winner)
    {
        yield return new WaitForSeconds(0.8f);
        _menuPanel.SetActive(false);
        _timingPanel.SetActive(false);
        _marker.gameObject.SetActive(false);

        bool win = winner == Team.Player;
        _resultText.text = win ? "VICTORY!" : "DEFEAT...";
        _resultText.color = win ? new Color(1f, 0.9f, 0.2f) : new Color(1f, 0.4f, 0.4f);
        _resultPanel.SetActive(true);

        yield return null;
        while (!PlayerInteractKey())
            yield return null;

        BattleContext.Finish(winner);
        SceneController.SwitchScene(BattleContext.ReturnScene);
    }


    // ------------------------------------------------------------ player command menu (IBattleMenu)

    public void RequestChoice(BattleUnit actor, IReadOnlyList<BattleUnit> all, Action<ActionChoice> onChosen)
    {
        StartCoroutine(MenuRoutine(actor, all, onChosen));
    }

    private IEnumerator MenuRoutine(BattleUnit actor, IReadOnlyList<BattleUnit> all, Action<ActionChoice> onChosen)
    {
        var skills = actor.Skills;
        int skillIndex = 0;
        SkillData skill = null;
        BattleUnit picked = null;

        _menuPanel.SetActive(true);

        while (true)
        {
            // 1. choose a skill
            _hintText.text = "W/S: select    E: confirm";
            while (true)
            {
                DrawSkillMenu(actor, skills, skillIndex);
                if (PlayerMenuUp()) skillIndex = (skillIndex - 1 + skills.Count) % skills.Count;
                else if (PlayerMenuDown()) skillIndex = (skillIndex + 1) % skills.Count;
                else if (PlayerInteractKey() && actor.Mp >= skills[skillIndex].MpCost) break;
                yield return null;
            }
            skill = skills[skillIndex];
            yield return null; // swallow the E press so it does not also pick the first target

            // 2. choose a target (only when there is something to choose)
            var candidates = BattleTargeting.GetEffectedCandidates(actor, skill, all);
            picked = candidates.Count > 0 ? candidates[0] : null;
            bool back = false;

            if (BattleTargeting.IsNeedPickingTarget(skill, candidates.Count))
            {
                int ti = 0;
                _hintText.text = "W/S/A/D: target    E: confirm    X: back";
                while (true)
                {
                    ShowMarker(candidates[ti]);
                    SetMenuText(actor.DisplayName() + "  >  " + skill.SkillName + "\nChoose target: " + candidates[ti].DisplayName());
                    if (PlayerMenuUp() || PlayerMenuLeft()) ti = (ti - 1 + candidates.Count) % candidates.Count;
                    else if (PlayerMenuDown() || PlayerMenuRight()) ti = (ti + 1) % candidates.Count;
                    else if (PlayerDialogEscapeKey()) { back = true; break; }
                    else if (PlayerInteractKey()) { picked = candidates[ti]; break; }
                    yield return null;
                }
                _marker.gameObject.SetActive(false);
                yield return null;
            }

            if (back) continue;
            break;
        }

        _menuPanel.SetActive(false);
        onChosen(new ActionChoice { Skill = skill, Target = picked });
    }

    private void DrawSkillMenu(BattleUnit actor, IReadOnlyList<SkillData> skills, int index)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine(actor.DisplayName() + "    MP " + actor.Mp + "/" + CharacterData.MaxMp);
        for (int i = 0; i < skills.Count; i++)
        {
            var s = skills[i];
            bool affordable = actor.Mp >= s.MpCost;
            string line = (i == index ? "> " : "   ") + s.SkillName + (s.MpCost > 0 ? "   (MP " + s.MpCost + ")" : "");
            Color c = !affordable ? Color.gray : (i == index ? new Color(1f, 0.9f, 0.2f) : Color.white);
            sb.AppendLine("<color=#" + ColorUtility.ToHtmlStringRGB(c) + ">" + line + "</color>");
        }
        SetMenuText(sb.ToString());
    }

    private void SetMenuText(string text)
    {
        if (_menuText.text != text) _menuText.text = text;
    }

    private void ShowMarker(BattleUnit unit)
    {
        if (!_views.TryGetValue(unit, out var view)) { return; }
        _marker.transform.SetParent(view.Root, false);
        var rt = _marker.rectTransform;
        rt.anchorMin = V(0.46f, 0.78f);
        rt.anchorMax = V(0.54f, 0.86f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        _marker.gameObject.SetActive(true);
    }
}
