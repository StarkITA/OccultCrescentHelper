using Action = System.Action;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using System.Collections.Generic;
using System.Linq;

namespace OccultCrescentHelper.Modules.StateManager;

public class StateManager
{
    private State state = State.Idle;

    public event Action? OnEnterIdle;
    public event Action? OnExitIdle;

    public event Action? OnEnterInCombat;
    public event Action? OnExitInCombat;

    public event Action? OnEnterInFate;
    public event Action? OnExitInFate;

    public event Action? OnEnterInCriticalEncounter;
    public event Action? OnExitInCriticalEncounter;

    private Dictionary<State, Action> handlers;

    private Dictionary<State, Action?> enter;

    private Dictionary<State, Action?> exit;

    public StateManager()
    {
        handlers = new() {
            {State.Idle, HandleIdle },
            {State.InCombat, HandleInCombat },
            {State.InFate, HandleInFate },
            {State.InCriticalEncounter, HandleInCriticalEncounter },
        };

        enter = new() {
            {State.Idle, OnEnterIdle },
            {State.InCombat, OnEnterInCombat },
            {State.InFate, OnEnterInFate },
            {State.InCriticalEncounter, OnEnterInCriticalEncounter },
        };

        exit = new() {
            {State.Idle, OnExitIdle },
            {State.InCombat, OnExitInCombat },
            {State.InFate, OnExitInFate },
            {State.InCriticalEncounter, OnExitInCriticalEncounter },
        };
    }

    public void Tick(IFramework _)
    {
        if (Svc.ClientState.LocalPlayer?.IsDead ?? true)
        {
            return;
        }

        handlers[state]();
    }


    private void HandleIdle()
    {
        if (IsInCombat())
        {
            ChangeState(State.InCombat);
            return;
        }

        if (IsInFate())
        {
            ChangeState(State.InFate);
            return;
        }

        if (IsInCriticalEncounter())
        {
            ChangeState(State.InCriticalEncounter);
            return;
        }
    }

    private void HandleInCombat()
    {
        if (IsInFate())
        {
            ChangeState(State.InFate);
            return;
        }

        if (IsInCriticalEncounter())
        {
            ChangeState(State.InCriticalEncounter);
            return;
        }


        if (!IsInCombat())
        {
            ChangeState(State.Idle);
            return;
        }
    }

    public void HandleInFate()
    {
        if (!IsInFate())
        {
            ChangeState(IsInCombat() ? State.InCombat : State.Idle);
            return;
        }
    }

    public void HandleInCriticalEncounter()
    {
        if (!IsInCriticalEncounter())
        {
            ChangeState(IsInCombat() ? State.InCriticalEncounter : State.Idle);
            return;
        }
    }

    private void ChangeState(State newState)
    {
        if (newState == state)
            return;

        var oldState = state;
        Svc.Log.Info($"[StateManager] State changed from {oldState} to {newState}");

        exit[oldState]?.Invoke();
        state = newState;
        enter[newState]?.Invoke();
    }


    private bool IsInCombat() => Svc.Condition[ConditionFlag.InCombat];

    private unsafe bool IsInFate() => FateManager.Instance()->CurrentFate is not null;

    // 1778 = Hoofing It (Unable to mount)
    private bool IsInCriticalEncounter() => Svc.ClientState.LocalPlayer?.StatusList.Any(s => s.StatusId == 1778) ?? false;

    public State GetState() => state;
}
