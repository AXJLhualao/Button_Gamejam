// 2026.6.29 --v1 
// 状态接口 —— 所有状态类的统一契约。
// 每个状态由 Enter / Update / Exit 三段生命周期组成：
//   Enter  ：进入状态时执行一次（初始化、播放动画）
//   Update ：每帧调用（处理逻辑、检测转移条件）
//   Exit   ：离开状态时执行一次（清理、重置）
// 用法：实现此接口并传入 StateMachine 即可被驱动。
// 另见 AnimatedState（装饰器）可在不修改原状态的前提下注入动画播放。
public interface IState
{
    void Enter();
    void Update();
    void Exit();
}
