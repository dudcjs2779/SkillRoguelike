using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomSlider : Slider
{
    enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }

    [SerializeField]
    private Direction _m_Direction = Direction.LeftToRight;
    
    Axis _axis { get { return (_m_Direction == Direction.LeftToRight || _m_Direction == Direction.RightToLeft) ? Axis.Horizontal : Axis.Vertical; } }
    bool _reverseValue { get { return _m_Direction == Direction.RightToLeft || _m_Direction == Direction.TopToBottom; } }
    float _stepSize { get { return wholeNumbers ? 1 : GameManager.Instance.Remap(PlayerInputControls.Instance.holdTime, 0, 1, minStep, maxStep); } }
    public float minStep = 0.1f;
    public float maxStep = 0.1f;

    public override void OnMove(AxisEventData eventData)
    {
        //base.OnMove(eventData);

        if (!IsActive() || !IsInteractable())
        {
            base.OnMove(eventData);
            return;
        }

        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
                if (_axis == Axis.Horizontal && FindSelectableOnLeft() == null)
                    Set(_reverseValue ? value + _stepSize : value - _stepSize);
                else
                    base.OnMove(eventData);
                break;
            case MoveDirection.Right:
                if (_axis == Axis.Horizontal && FindSelectableOnRight() == null)
                    Set(_reverseValue ? value - _stepSize : value + _stepSize);
                else
                    base.OnMove(eventData);
                break;
            case MoveDirection.Up:
                if (_axis == Axis.Vertical && FindSelectableOnUp() == null)
                    Set(_reverseValue ? value - _stepSize : value + _stepSize);
                else
                    base.OnMove(eventData);
                break;
            case MoveDirection.Down:
                if (_axis == Axis.Vertical && FindSelectableOnDown() == null)
                    Set(_reverseValue ? value + _stepSize : value - _stepSize);
                else
                    base.OnMove(eventData);
                break;
        }

    }
}
