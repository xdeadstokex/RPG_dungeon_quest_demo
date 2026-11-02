using System.Collections.Generic;
using Assets.Scripts.Battle;
using UnityEngine;
using static GameData;

public class CollisionHandler : MonoBehaviour
{
    public enum TriggerType { Click, Enter } 

    [SerializeField] private SceneNames _targetScene = SceneNames.None;
    [SerializeField] private ZoneType _zoneType = ZoneType.Portal;
    [SerializeField] private TriggerType _triggerType = TriggerType.Enter;
    [SerializeField] private CharacterData[] _enemies;
    //must change to just id then load enemies data from db with this id

    public static Collider2D _lastHit;


    private void Start()
    {
        if (_zoneType != ZoneType.Fight)
            return;

        //disable this CollisionHandler if it is defeated
        string id = BattleContext.ZoneId(SceneUtils.GetCurrentSceneName(), name);
        if (BattleContext.ClearedZones.Contains(id))
            gameObject.SetActive(false);
    }


    public static GameObject GetMouseTarget(LayerMask layer)
    {
        var mouseWorldPos = MouseInputUtils.MouseWorldPos2D();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, layer);
  
        return (hit == null) ? null : hit.gameObject;
    }


    public static bool IsInZoneType(Vector2 playerPos, PlayerData data, LayerMask layer, ZoneType type)
    {
        var playerBoundBox = data.boxCollider.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(playerPos, playerBoundBox, 0f, layer);
        Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);

        if (hitObject == null)
            return false;

        if (!hitObject.TryGetComponent<CollisionHandler>(out var dataScript))
            return false;

        return type == dataScript._zoneType;
    }

    public static string GetZoneName(PlayerData data, LayerMask layer)
    {
        var playerPos = data.transform.position;
        Collider2D hitObject = Physics2D.OverlapCircle(playerPos, 0.5f, layer);
        return (hitObject != null) ? hitObject.gameObject.name : null;
    }

    public static void TriggerZoneFunction(PlayerData data, LayerMask layer)
    {
        var playerPos = data.transform.position;
        var playerBoundBox = data.boxCollider.bounds.size;
        Collider2D[] hits = Physics2D.OverlapBoxAll(playerPos, playerBoundBox, 0f, layer);
        Collider2D hitObject = System.Array.Find(hits, h => h.gameObject != data.obj);


        if (hitObject == null) { goto end; }
        bool inside = hitObject != null;

        if (!hitObject.TryGetComponent<CollisionHandler>(out var dataScript)) { goto end; }

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
        if (obj == null)
            return;

        if (obj.TryGetComponent<PopupHandler>(out var popup))
        {
            popup.FollowObj(data.transform, signal);
            popup.FollowPlayer(data, signal);
            popup.Show(signal);
        }
    }


    private static void HandleDialog(PlayerData data, CollisionHandler dataScript, Collider2D obj)
    {
        if (obj == null)
            return;

        if (dataScript._zoneType != ZoneType.Dialog)
            return;

        if (!PlayerInteractKey())
            return;

        if (obj.TryGetComponent<DialogHandler>(out var dialog))
            dialog.Show(data);
    }



    public static void StartFight(CollisionHandler zone)
    {
        if (Time.time < BattleContext.FightCooldownUntil)
            return;

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

        //write a func maybe in scenecontroller (load party/enemies data -> switch to fightscene)
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