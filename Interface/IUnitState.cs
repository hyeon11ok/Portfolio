public interface IUnitState
{
    void StateEnter();
    void StateExit();
    void StateUpdate();
    void StatePhysicsUpdate();
}
