using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class InteractableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    [FormerlySerializedAs("selectSound")]
    [SerializeField] SoundManager.UISFXType selectSound;
    [FormerlySerializedAs("selectName")]
    [SerializeField] string selectName;

    [Space(10f)]
    [FormerlySerializedAs("clickSound")]
    [SerializeField] SoundManager.UISFXType clickSound;
    [FormerlySerializedAs("clickName")]
    [SerializeField] string clickName;

    private Selectable selectable;

    public bool isDebug;

    private void Awake() {
        LoadType();
        ClickSound();
        selectable = GetComponent<Selectable>();
    }

    void ClickSound(){
        Button button = GetComponent<Button>();
        if (button != null && clickSound != SoundManager.UISFXType.None)
        {
            button.onClick.AddListener(() => { SoundManager.Instance.PlayUISound(clickSound); });
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(selectable.IsInteractable()){
            GameManager.Instance.SelectCursor();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Instance.DefaultCursor();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selectSound != SoundManager.UISFXType.None)
            SoundManager.Instance.PlayUISound(selectSound);
    }

    [ContextMenu("Save Type")]
    public void SaveType()
    {
        selectName = selectSound.ToString();
        clickName = clickSound.ToString();
    }

    [ContextMenu("Load Type")]
    public void LoadType(){
        try
        {
            SoundManager.UISFXType type = (SoundManager.UISFXType)System.Enum.Parse(typeof(SoundManager.UISFXType), selectName);
            selectSound = type;

            type = (SoundManager.UISFXType)System.Enum.Parse(typeof(SoundManager.UISFXType), clickName);
            clickSound = type;
        }
        catch
        {
            selectSound = SoundManager.UISFXType.None;
            clickSound = SoundManager.UISFXType.None;
        }
    }

    
}
