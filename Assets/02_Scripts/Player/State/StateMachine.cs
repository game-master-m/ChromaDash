using System;
using System.Collections.Generic;

public class StateMachine
{
    public IState CurrentState { get; private set; }
    private Dictionary<IState, List<Transition>> transitionDic = new Dictionary<IState, List<Transition>>();
    private List<Transition> anyTransitions = new List<Transition>();

    public void Update()
    {
        Transition transition = GetValidTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }
        CurrentState?.Update();
    }
    public void FixedUpdate()
    {
        CurrentState?.FixedUpdate();
    }
    public void ChangeState(IState nextState)
    {
        if (CurrentState != null) CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void AddTransition(IState from, IState to, Func<bool> condition)
    {
        if (!transitionDic.ContainsKey(from)) transitionDic[from] = new List<Transition>();
        transitionDic[from].Add(new Transition(to, condition));
    }
    public void AddAnyTransition(IState to, Func<bool> condition)
    {
        anyTransitions.Add(new Transition(to, condition));
    }
    private Transition GetValidTransition()
    {
        //���� return ������ ������ �켱������ PlayerController���� �־��ִ´�� ��
        if (anyTransitions.Count != 0 && anyTransitions != null)
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Invoke())
                {
                    return transition;
                }
            }
        }
        //�˻� �� ��ȯ������ ���� ��, �θ��� ��ȯ���ǵ� �˻�
        IState state = CurrentState;

        while (state != null && state is PlayerState)
        {
            if (transitionDic.ContainsKey(state))
            {
                foreach (var transition in transitionDic[state])
                {
                    if (transition.Condition.Invoke())
                    {
                        return transition;
                    }
                }
            }
            state = (state as PlayerState).Parent;
        }
        return null;
    }
    private class Transition
    {
        public IState To { get; }
        public Func<bool> Condition { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }

    }
}
