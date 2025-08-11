using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    private UIManager uIManager;
    private DialogueManager dialogueManager;

    public int ID;
    [SerializeField] private string interactionPrompt = "[F] 대화하기";
    [SerializeField] private Transform promptPivot;

    public string InteractionPrompt => interactionPrompt;

    public Transform PromptPivot => promptPivot;

    private void Start()
    {
        uIManager = UIManager.Instance;
        dialogueManager = DialogueManager.Instance;
    }
    public bool CanInteract(GameObject interactor)
    {
        return true;
    }

    public void Interact(GameObject interactor)
    {
        var npcDataTable = TableManager.Instance.GetTable<NPCDataTable>();
        var npcData = npcDataTable.GetDataByID(ID);

        if (npcData == null)
        {
            Debug.LogWarning($"[NPCController] ID {ID}에 해당하는 NPCData를 찾을 수 없습니다.");
            return;
        }

        switch (npcData.Type)
        {
            case NPCType.Normal:
                OpenDialogue(interactor);
                break;
            case NPCType.Merchant:
                TryOpenShop();
                break;
            case NPCType.Enhancer:
                HandleEnhancerInteraction();
                break;
            case NPCType.Healer:
                HandleHealerInteraction();
                break;
        }
    }
    private void OpenDialogue(GameObject interactor)
    {
        uIManager.GetUI<DialogueBoard>().Init(TableManager.Instance.GetTable<NPCDataTable>().GetDataByID(ID).description);
        uIManager.ShowUI<DialogueBoard>();        
        dialogueManager.onDialogue(interactor.transform, this.transform);
    }
    private void TryOpenShop()
    {
        if (!TryGetComponent(out ShopInventory shopInventory))
        {
            Debug.LogWarning($"[NPCController] 이 NPC({ID})에 ShopInventory 컴포넌트가 없습니다.");
            return;
        }

        if (!UIManager.Instance.TryGetUI<ShopUI>(out var shopUI))
        {
            Debug.LogError("[NPCController] ShopUI를 찾을 수 없습니다.");
            return;
        }

        if (!shopInventory.Initialized)
        {
            shopInventory.OnInitialized += () => shopUI.OpenWithInventory(shopInventory);
        }
        else if(!shopUI.gameObject.activeSelf)
        {
            shopUI.OpenWithInventory(shopInventory);
        }
    }
    private void HandleEnhancerInteraction()
    {
        if (!UIManager.Instance.TryGetUI<EnhanceBoard>(out var enhanceBoard))
        {
            Debug.LogError("[NPCController] EnhanceBoard를 찾을 수 없습니다.");
            return;
        }

        if(GameManager.Instance != null && GameManager.Instance.GetNpcInteractionProcessed(this.gameObject))
        {
            UIManager.Instance.ShowConfirmPopup(
                "이미 강화를 완료했습니다",
                onConfirm: () => { },
                onCancel: null,
                confirmText: "확인(Enter)"
            );
        }
        else if(!enhanceBoard.gameObject.activeSelf)
        {
            enhanceBoard.Open(this.gameObject);
        }
    }

    private void HandleHealerInteraction()
    {
        if (!UIManager.Instance.TryGetUI<HealUI>(out var healUI))
        {
            Debug.LogError("[NPCController] EnhanceBoard를 찾을 수 없습니다.");
            return;
        }
        
        if(GameManager.Instance != null && GameManager.Instance.GetNpcInteractionProcessed(this.gameObject))
        {
            UIManager.Instance.ShowConfirmPopup(
                "이미 치료를 완료했습니다",
                onConfirm: () => { },
                onCancel: null,
                confirmText: "확인(Enter)"
            );
        }
        else if(!healUI.gameObject.activeSelf)
        {
            healUI.Open(this.gameObject);
        }
    }
}
