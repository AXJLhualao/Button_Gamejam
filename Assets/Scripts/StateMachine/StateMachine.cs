// 2026.6.29 --v1 
// 核心状态机 —— 纯 C# 类，不依赖 MonoBehaviour，所有项目均可直接使用。
// 职责：管理当前状态，驱动 Enter / Update / Exit 生命周期。
// 用法：
//   1. new StateMachine()
//   2. machine.Initialize(startState)
//   3. 在 MonoBehaviour.Update 中调用 machine.Update()
//   4. 状态内部通过 machine.TransitionTo(nextState) 转移
// 注意：TransitionTo 会跳过对同一状态的重复转移
//       （若 nextState == currentState 则不操作）
public class StateMachine
{
    private IState currentState;

    public void Initialize(IState startingState)
    {
        currentState = startingState;
        currentState?.Enter();
    }

    public void TransitionTo(IState nextState)
    {
        if (nextState == currentState) return;

        currentState?.Exit();
        currentState = nextState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
