using Action = System.Action;
using System.Text.RegularExpressions;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;

namespace OccultCrescentHelper.Modules.StateManager;

public class StateManager
{
    private State state = State.Idle;

    private bool combatFlag = false;

    public event Action? OnEnterIdle;
    public event Action? OnExitIdle;

    public event Action? OnEnterInCombat;
    public event Action? OnExitInCombat;

    public event Action? OnEnterInFate;
    public event Action? OnExitInFate;

    public event Action? OnEnterInCriticalEncounter;
    public event Action? OnExitInCriticalEncounter;

    public void Tick(IFramework _)
    {
        if (Svc.ClientState.LocalPlayer?.IsDead ?? true)
        {
            return;
        }

        if (state == State.Idle)
        {
            if (!IsInCombat())
            {
                return;
            }

            if (IsInFate())
            {
                ChangeState(State.InFate);
                return;
            }

            ChangeState(State.InCombat);
            return;
        }

        if (state == State.InFate)
        {
            if (IsInFate())
            {
                return;
            }

            if (IsInCombat())
            {
                ChangeState(State.InCombat);
                return;
            }

            ChangeState(State.Idle);
            return;
        }

        if (state == State.InCombat)
        {
            if (IsInCombat())
            {
                return;
            }

            if (IsInFate())
            {
                ChangeState(State.InFate);
                return;
            }

            ChangeState(State.Idle);
            return;
        }

        if (state == State.InCriticalEncounter)
        {
            if (!combatFlag)
            {
                combatFlag = IsInCombat();
                return;
            }

            if (IsInCombat())
            {
                return;
            }

            combatFlag = false;
            ChangeState(State.Idle);
            return;
        }
    }

    public void OnChatMessage(XivChatType type, int timestamp, SeString sender, SeString message, bool isHandled)
    {
        if (state != State.Idle || type != XivChatType.SystemMessage)
        {
            return;
        }

        const string criticalEngagementStartingPattern =
            "You have joined the critical encounter|Vous participez à la confrontation critique|Du nimmst an der kritischen Begegnung|以下のクリティカルエンカウントに参加しました。";

        if (Regex.IsMatch(message.ToString(), criticalEngagementStartingPattern))
        {
            ChangeState(State.InCriticalEncounter);
        }
    }


    private void ChangeState(State newState)
    {
        if (newState == state)
            return;

        var oldState = state;
        Svc.Log.Info($"[StateManager] State changed from {oldState} to {newState}");

        switch (oldState)
        {
            case State.Idle:
                OnExitIdle?.Invoke();
                break;
            case State.InCombat:
                OnExitInCombat?.Invoke();
                break;
            case State.InFate:
                OnExitInFate?.Invoke();
                break;
            case State.InCriticalEncounter:
                OnExitInCriticalEncounter?.Invoke();
                break;
        }

        state = newState;

        switch (newState)
        {
            case State.Idle:
                OnEnterIdle?.Invoke();
                break;
            case State.InCombat:
                OnEnterInCombat?.Invoke();
                break;
            case State.InFate:
                OnEnterInFate?.Invoke();
                break;
            case State.InCriticalEncounter:
                OnEnterInCriticalEncounter?.Invoke();
                break;
        }
    }

    private unsafe bool IsInFate() => FateManager.Instance()->CurrentFate is not null;

    private bool IsInCombat() => Svc.Condition[ConditionFlag.InCombat];

    public State GetState() => state;
}
