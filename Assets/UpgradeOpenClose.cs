using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeOpenClose : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UnityEvent _onOpen;
    [SerializeField] private UnityEvent _onClose;
    private bool _opened;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _opened = !_opened;
        if (_opened)
        {
            _onOpen.Invoke();
        }
        else
        {
            _onClose.Invoke();
        }
    }
}
