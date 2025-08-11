
public class EnemyAnimationEventSender : AnimationEventSender<EnemyController>
{
    public override void SendEvent()
    {
        originObject.AttackAction();
        if(originObject.soundName != null) SoundManager.Instance.PlaySFX(this.transform.position, originObject.soundName);

    }
}
