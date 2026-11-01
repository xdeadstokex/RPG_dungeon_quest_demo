using System.Collections.Generic;
using Assets.Scripts.Battle;
using UnityEngine;
using static GameData;

public class CollisionHandler : MonoBehaviour
{
    public enum TriggerType { Click, Enter } //should be moved

    [SerializeField] private SceneNames _targetScene = SceneNames.None;
    [SerializeField] private ZoneType _zoneType = ZoneType.Portal;
    [SerializeField] private TriggerType _triggerType = TriggerType.Enter;
    [SerializeField] private CharacterData[] _enemies; // used by ZoneType.Fight

    public static Collider2D _lastHit;


    private void Start()
    {
        // a Fight zone that was already won stays disabled
        if (_zoneType != ZoneType.Fight) { return; }
        string id = BattleContext.ZoneId(SceneUtils.GetCurrentSceneName(), name);
        if (BattleContext.ClearedZones.Contains(id))
        {
            gameObject.SetActive(false);
        }
    }


    public static GameObject GetMouseTarget(LayerMask layer)
    {
        Collider2D hit = Physics2D.OverlapPoint(MouseInputUtils.MouseWorldPos2D(), layer);
        if (hit == null)
            return null;

        Debug.Log("ol");
        return hit.gameObject;
    }


    public static bool IsInZoneType(Vector2 playerPos, PlayerData data, LayerMask layer, ZoneType type)
    {
        //var 
        Collider2D[] hits = Physics2D.OverlapBoxAll(playerPos, data.boxCollider.bounds.size, 0f, layer);
        Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);

        if (hitObject == null)
            return false;

        if (!hitObject.TryGetComponent<CollisionHandler>(out var dataScript))
            return false;

        return type == dataScript._zoneType;
    }

    public static string GetZoneName(PlayerData data, LayerMask layer)
    {
        Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);
        if (hitObject == null) { return null; }
        return hitObject.gameObject.name;
    }

    public static void TriggerZoneFunction(PlayerData data, LayerMask layer)
    {
        //Collider2D hitObject = Physics2D.OverlapCircle(data.transform.position, 0.5f, layer);

        Collider2D[] hits = Physics2D.OverlapBoxAll(data.transform.position, data.boxCollider.bounds.size, 0f, layer);
        Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);


        if (hitObject == null) { goto end; }
        bool inside = hitObject != null;

        CollisionHandler dataScript = hitObject.GetComponent<CollisionHandler>();
        if (dataScript == null) { goto end; }

        HandlePopup(data, hitObject, true);
        HandleDialog(data, dataScript, hitObject);

        switch (dataScript._zoneType)
        {

            case ZoneType.Portal:
                if (inside)
                {
                    if (dataScript._triggerType == TriggerType.Click && !PlayerInteractKey())
                    {
                        break;
                    }
                    CheckChangeScene(dataScript);
                }
                break;

            case ZoneType.Fight:
                if (inside)
                {
                    if (dataScript._triggerType == TriggerType.Click && !PlayerInteractKey())
                    {
                        break;
                    }
                    StartFight(dataScript);
                }
                break;


        }

    end:
        if (_lastHit != hitObject)
        {
            HandlePopup(data, _lastHit, false);
            _lastHit = hitObject;
        }
        return;
    }


    private static void HandlePopup(PlayerData data, Collider2D obj, bool signal)
    {
        if (obj == null) { return; }
        PopupHandler popup = obj.GetComponent<PopupHandler>();
        if (popup != null)
        {
            popup.FollowObj(data.transform, signal);
            popup.FollowPlayer(data, signal);
            popup.Show(signal);
        }
    }


    private static void HandleDialog(PlayerData data, CollisionHandler dataScript, Collider2D obj)
    {
        if (obj == null) { return; }
        if (dataScript._zoneType != ZoneType.Dialog) { return; }
        if (!PlayerInteractKey()) { return; }

        DialogHandler dialog = obj.GetComponent<DialogHandler>();
        if (dialog != null)
        {
            dialog.Show(data);
        }
    }



    public static void StartFight(CollisionHandler zone)
    {
        if (Time.time < BattleContext.FightCooldownUntil) { return; }
        if (zone._enemies == null || zone._enemies.Length == 0)
        {
            Debug.LogWarning("Fight zone '" + zone.name + "' has no enemies assigned");
            return;
        }

        List<CharacterData> party = InputController.GetPartyCharacters();
        if (party.Count == 0)
        {
            Debug.LogWarning("No party member has a CharacterData assigned (EntityHandler -> Character)");
            return;
        }

        string current = SceneUtils.GetCurrentSceneName();
        BattleContext.Set(party, new List<CharacterData>(zone._enemies));
        BattleContext.ReturnScene = current;
        BattleContext.CurrentId = BattleContext.ZoneId(current, zone.name);
        SceneController.SwitchScene(SceneNames.FightScene.ToString());
    }


    public static void CheckChangeScene(CollisionHandler dataScript)
    {
        string sceneName = dataScript._targetScene.ToString();
        if (sceneName == "None") { return; }
        SceneController.SwitchScene(sceneName);
        return;
    }


}