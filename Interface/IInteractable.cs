using UnityEngine;

public interface IInteractable 
{
    string InteractionPrompt { get; } // 상호작용 힌트 텍스트 (예: "E키를 눌러 열기")
    Transform PromptPivot { get; }

    void Interact(GameObject interactor); // 상호작용 시 실행
    bool CanInteract(GameObject interactor); // 상호작용 가능한 상태인지 여부
}
